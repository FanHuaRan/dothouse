using DotHouse.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHouse.Domain
{
    /// <summary>
    /// 二手房
    /// </summary>
    public class House
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public Decimal UnitPrice { get; set; }

        /// <summary>
        /// 面积
        /// </summary>
        public Decimal  Dimensions  { get; set; }

       /// <summary>
       /// 户型
       /// </summary>
        public HouseTypeEnum HouseType { get; set; }

        /// <summary>
        /// 用途编号
        /// </summary>
        public Int32 UsageId { get; set; }

        /// <summary>
        /// 用途名称
        /// </summary>
        public String UsageName { get; set; }

        /// <summary>
        /// 楼层
        /// </summary>
        public Int32 Floor { get; set; }

        /// <summary>
        /// 朝向
        /// </summary>
        public FaceEnum Face { get; set; }

        /// <summary>
        /// 建成日期
        /// </summary>
        public Int32 StartYear { get; set; }

        /// <summary>
        /// 是否有电梯，1代表有，0代表无
        /// </summary>
        public Int32 HasElevator { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double y { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
