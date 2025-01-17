﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedId
{
    /// <summary>
    /// 分布式Id
    /// </summary>
    public interface IDistributedId
    {
        /// <summary>
        /// 生成有序Guid,使用默认排序类型
        /// </summary>
        /// <returns></returns>
        Guid NewGuid();
        /// <summary>
        /// 生成有序Guid,指定排序类型
        /// </summary>
        /// <param name="sequentialGuidType">排序类型</param>
        /// <returns></returns>
        Guid NewGuid(SequentialGuidType sequentialGuidType);
        /// <summary>
        /// 生成有序雪花Id
        /// </summary>
        /// <returns></returns>
        long NewLongId();
    }
}
