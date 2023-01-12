using MISA.WEB09.QLTS.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.Common.Entities
{
    /// <summary>
    /// Ngân sách
    /// </summary>
    public class Budget : BaseEntity
    {
        /// <summary>
        /// Id ngân sách
        /// </summary>
        [PrimaryKey]
        public Guid budget_id { get; set; }

        /// <summary>
        /// Tên ngân sách
        /// </summary>
        public string budget_name { get; set; }
    }
}
