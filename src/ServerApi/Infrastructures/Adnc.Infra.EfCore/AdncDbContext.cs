﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JetBrains.Annotations;
using Adnc.Core.Shared;
using Adnc.Core.Shared.Entities;

namespace Adnc.Infra.EfCore
{
    public class AdncDbContext : DbContext
    {
        private readonly IOperater _operater;
        private readonly IEntityInfo _entityInfo;
        private readonly UnitOfWorkStatus _unitOfWorkStatus;

        public AdncDbContext([NotNull] DbContextOptions options, IOperater operater, [NotNull] IEntityInfo entityInfo, UnitOfWorkStatus unitOfWorkStatus)
            : base(options)
        {
            _operater = operater;
            _entityInfo = entityInfo;
            _unitOfWorkStatus = unitOfWorkStatus;

            //关闭DbContext默认事务
            Database.AutoTransactionsEnabled = false;
            //关闭查询跟踪
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var changedEntities = this.SetAuditFields();

            //没有自动开启事务的情况下,保证主从表插入，主从表更新开启事务。
            var isManualTransaction = false;
            if (!Database.AutoTransactionsEnabled && !_unitOfWorkStatus.IsStartingUow && changedEntities > 1)
            {
                isManualTransaction = true;
                Database.AutoTransactionsEnabled = true;
            }

            var result = base.SaveChangesAsync(cancellationToken);

            //如果手工开启了自动事务，用完后关闭。
            if (isManualTransaction)
                Database.AutoTransactionsEnabled = false;

            return result;
        }

        private int SetAuditFields()
        {
            var allEntities = ChangeTracker.Entries<Entity>();

            var allBasicAuditEntities = ChangeTracker.Entries<IBasicAuditInfo>().Where(x => x.State == EntityState.Added);
            foreach (var entry in allBasicAuditEntities)
            {
                var entity = entry.Entity;
                {
                    entity.CreateBy = _operater.Id;
                    entity.CreateTime = DateTime.Now;
                }
            }

            var auditFullEntities = ChangeTracker.Entries<IFullAuditInfo>().Where(x => x.State == EntityState.Modified);
            foreach (var entry in auditFullEntities)
            {
                var entity = entry.Entity;
                {
                    entity.ModifyBy = _operater.Id;
                    entity.ModifyTime = DateTime.Now;
                }
            }

            return allEntities.Count();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var (Assembly, Types) = _entityInfo.GetEntitiesInfo();

            foreach (var entityType in Types)
            {
                modelBuilder.Entity(entityType);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //用于设置是否启用缓存,关闭缓存，每次都会调用OnModelCreating。optionsBuilder.EnableServiceProviderCaching(false);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
