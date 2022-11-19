using Dapper;
using MISA.WEB09.QLTS.Common.Entities;
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

        #region API PagedingFillter

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
        #endregion


        #region API NextCode
        /// <summary>
        /// Sinh mã tài sản tiếp theo
        /// </summary>
        /// <returns>Mã tài sản tiếp theo</returns>
        /// Cretaed by: NNNINH (09/11/2022)
        public string NextAssetCode()
        {
            // Khai báo tên prodecure Insert
            string storedProcedureName = "Proc_asset_GetNextCode";

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            string nextAssetCode = "";
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                nextAssetCode = mysqlConnection.QueryFirstOrDefault<string>(storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);
            }
            // Xử lý dữ liệu trả về
            if (nextAssetCode != null)
            {
                return nextAssetCode;
            }
            else
            {
                return "";
            }
        } 
        #endregion
    }
}
