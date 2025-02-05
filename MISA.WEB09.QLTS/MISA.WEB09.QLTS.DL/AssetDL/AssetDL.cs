﻿using Dapper;
using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.Common.Enums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.DL
{
    public class AssetDL :BaseDL<Asset>, IAssetDL
    {

        /// <summary>
        /// Lấy danh sách các tài sản có chọn lọc
        /// </summary>
        /// <param name="keyword">Từ để tìm kiếm theo mã và tên tài sản</param>
        /// <param name="departmentId">ID phòng ban</param>
        /// <param name="categoryId">ID loại tài sản</param>
        /// <param name="limit">Số bản ghi muốn lấy</param>
        /// <param name="page">Số trang bắt đầu lấy</param>
        /// <returns>Danh sách các tài sản sau khi chọn lọc và các giá trị khác</returns>
        /// Created by: NNNINH (12/11/2022)
        public PagingData<Asset> FilterAssets(PagingAsset pagingAsset)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            parameters.Add("v_Offset", (pagingAsset.page - 1) * pagingAsset.limit);
            parameters.Add("v_Limit", pagingAsset.limit);
            parameters.Add("v_Sort", "");

            var whereConditions = new List<string>();
            var listDepartmentId = new List<string>();
            var listCategoryId = new List<string>();
            if (pagingAsset.keyword != null) whereConditions.Add($"(fixed_asset_code LIKE \'%{pagingAsset.keyword}%\' OR fixed_asset_name LIKE \'%{pagingAsset.keyword}%\')");
            if (pagingAsset.listDepartment.Count != 0)
            {
                foreach (var department in pagingAsset.listDepartment)
                {
                    listDepartmentId.Add($"department_id LIKE \'{department}\'");
                }
                string whereDepartmentId = string.Join(" OR ", listDepartmentId);
                whereConditions.Add(whereDepartmentId);
            }
            if (pagingAsset.listCategory.Count != 0)
            {
                foreach (var category in pagingAsset.listCategory)
                {
                    listCategoryId.Add($"fixed_asset_category_id LIKE \'{category}\'");
                }
                string whereCategoryId = string.Join(" OR ", listCategoryId);
                whereConditions.Add(whereCategoryId);
            }
            if (pagingAsset.listIdAsset != null) {
                /// Lấy các bản ghi được chọn
                if (pagingAsset.mode == (int)GetRecordMode.Selected)
                {
                    whereConditions.Add($"fixed_asset_id IN ('{String.Join("','", pagingAsset.listIdAsset)}')");
                }
                /// Lấy các bản ghi không được chọn và không ghi tăng
                else if (pagingAsset.mode == (int)GetRecordMode.NotSelectedNotIncrement)
                {
                    whereConditions.Add($"fixed_asset_id NOT IN ('{String.Join("','", pagingAsset.listIdAsset)}') AND increment_status = 0 OR increment_status IS NULL");
                }
                /// Lấy các bản ghi không được chọn
                else
                {
                    whereConditions.Add($"fixed_asset_id NOT IN ('{String.Join("','", pagingAsset.listIdAsset)}')");
                }
            }
            string whereClause = string.Join(" AND ", whereConditions);

            parameters.Add("v_Where", whereClause);

            // Khai báo tên prodecure Insert
            string storedProcedureName = "Proc_asset_GetPaging";

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            var filterResponse = new PagingData<Asset>();
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Thực hiện gọi vào DB để chạy procedure
                var multiAssets = mysqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Xử lý dữ liệu trả về
                var assets = multiAssets.Read<Asset>();
                var totalCount = multiAssets.Read<long>().Single();
                var totalQuantity = multiAssets.Read<long>().Single();
                var totalCost = multiAssets.Read<double>().Single();
                var totalDepreciation = multiAssets.Read<double>().Single();
                var totalRemain = multiAssets.Read<double>().Single();

                filterResponse = new PagingData<Asset>(assets, totalCount, totalQuantity, totalCost, totalDepreciation, totalRemain);
            }

            return filterResponse;
        }
    }
}
