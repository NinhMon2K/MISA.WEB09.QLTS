using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.WEB09.QLTS.Common.Attributes;

namespace MISA.WEB09.QLTS.Common.Entities
{
    /// <summary>
    /// Phòng ban
    /// </summary>
    public class Department : BaseEntity
    {
        /// <summary>
        /// ID phòng ban
        /// </summary>
        [PrimaryKey]
        public Guid department_id { get; set; }

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        public string department_code { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        public string department_name { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Có phải lớp cha không
        /// </summary>
        public string is_parent { get; set; }

        /// <summary>
        /// ID phòng ban cha
        /// </summary>
        public string parent_id { get; set; }

        /// <summary>
        /// ID đơn vị
        /// </summary>
        public string organization_id { get; set; }
    }
}
