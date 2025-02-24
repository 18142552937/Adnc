﻿using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Xunit;
using Xunit.Abstractions;
using Adnc.Infra.Common.Helper;
using Adnc.Infra.Common.Helper.IdGeneraterInternal;

namespace Adnc.UnitTest.Helper
{
    public class IdGeneraterTests
    {
        private ITestOutputHelper _output;
        public IdGeneraterTests(ITestOutputHelper output)
        {
            _output = output;
            if (YitterSnowFlake.CurrentWorkerId < 0)
                YitterSnowFlake.CurrentWorkerId = 63;
        }

        /// <summary>
        /// id 小于 js 最大值 151599672553541
        /// </summary>
        [Fact]
        public void TestIdLessThanjJsMaxNumber()
        {
            long id = IdGenerater.GetNextId();
            _output.WriteLine(id.ToString());
            Assert.True(9007199254740992 > id);
        }

        /// <summary>
        /// 100W个ids,没有重复
        /// </summary>
        [Fact]
        public void TestNotContainsDuplicateIds()
        {
            var set = new HashSet<long>();
            for (int index = 0; index < 100000; index++)
            {
                long id01 = IdGenerater.GetNextId();
                set.Add(id01);
            }
            Assert.Equal(100000, set.Count());
        }

        /// <summary>
        /// 100W个ids,517毫秒
        /// </summary>
        [Fact]
        public void TestSpeed()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            Parallel.For(1, 1000000, index =>
            {
                IdGenerater.GetNextId();
            });
            stopwatch.Stop();
            //持续时间: 517 毫秒
            _output.WriteLine(stopwatch.ElapsedMilliseconds.ToString());
            stopwatch.Reset();
        }
    }
}
