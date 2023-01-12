using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.WEB09.QLTS.Common.Attributes;

namespace MISA.WEB09.QLTS.Common.Entities
{
    /// <summary>
    /// Tài sản
    /// </summary>
    public class Asset : BaseEntity
    {

        /// <summary>
        /// ID tài sản
        /// </summary>
        [PrimaryKey]
        [IsNotNullOrEmpty("ID tài sản không được để trống")]
        public Guid fixed_asset_id { get; set; }

        /// <summary>
        /// Mã tài sản
        /// </summary>
        [IsNotNullOrEmpty("Mã tài sản không được để trống")]
        [IsNotDuplicate("Mã tài sản không được trùng")]
        public string fixed_asset_code { get; set; }

        /// <summary>
        /// Tên tài sản
        /// </summary>
        [IsNotNullOrEmpty("Tên tài sản không được để trống")]
        public string fixed_asset_name { get; set; }

        /// <summary>
        /// ID đơn vị
        /// </summary>
        public string? organization_id { get; set; }

        /// <summary>
        /// Mã đơn vị
        /// </summary>
        public string? organization_code { get; set; }

        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string? organization_name { get; set; }

        /// <summary>
        /// ID phòng ban
        /// </summary>
        [IsNotNullOrEmpty("ID phòng ban không được để trống")]
        public Guid department_id { get; set; }

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        [IsNotNullOrEmpty("Mã phòng ban không được để trống")]
        public string? department_code { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [IsNotNullOrEmpty("Tên phòng ban không được để trống")]
        public string? department_name { get; set; }

        /// <summary>
        /// ID loại tài sản
        /// </summary>
        [IsNotNullOrEmpty("ID loại tài sản không được để trống")]
        public Guid fixed_asset_category_id { get; set; }

        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        [IsNotNullOrEmpty("Mã loại tài sản không được để trống")]
        public string? fixed_asset_category_code { get; set; }

        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        [IsNotNullOrEmpty("Tên loại tài sản không được để trống")]
        public string? fixed_asset_category_name { get; set; }

        /// <summary>
        /// Ngày mua
        /// </summary>
        [IsNotNullOrEmpty("Ngày mua không được để trống")]
        public DateTime purchase_date { get; set; }

        /// <summary>
        /// Nguyên giá
        /// </summary>
        [IsNotNullOrEmpty("Nguyên giá không được để trống")]
        public double cost { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        [IsNotNullOrEmpty("Số lượng không được để trống")]
        public int quantity { get; set; }

        /// <summary>
        /// Tỉ lệ hao mòn (%)
        /// </summary>
        [IsNotNullOrEmpty("Tỉ lệ hao mòn không được để trống")]
        public double depreciation_rate { get; set; }

        /// <summary>
        /// Năm theo dõi
        /// </summary>
        [IsNotNullOrEmpty("Năm theo dõi không được để trống")]
        public int tracked_year { get; set; }

        /// <summary>
        /// Số năm sử dụng
        /// </summary>
        [IsNotNullOrEmpty("Số năm sử dụng không được để trống")]
        public int life_time { get; set; }

        /// <summary>
        /// Năm sử dụng
        /// </summary>
        public int production_year { get; set; }

        /// <summary>
        /// Ngày bắt đầu sử dụng
        /// </summary>
        [IsNotNullOrEmpty("Ngày bắt đầu sử dụng không được để trống")]
        public DateTime production_date { get; set; }

        /// <summary>
        /// Còn hoạt động hay không
        /// </summary>
        public Boolean active { get; set; }

        /// <summary>
        /// Hao mòn năm
        /// </summary>
        [IsNotNullOrEmpty("Hao mòn năm không được để trống")]
        public double depreciation_year { get; set; }


        /// <summary>
        /// Trạng thái ghi tăng
        /// </summary>
        public Boolean increment_status { get; set; }

        /// <summary>
        /// Nguồn ngân sách tài sản
        /// </summary>
        public string? budget { get; set; }

        public double? oldCost { get; set; }

        public int? flag { get; set; }
    }
}
