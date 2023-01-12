using Dapper;
using MISA.WEB09.QLTS.Common.Attributes;
using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.Common.Resources;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.DL
{
    public class VoucherDL : BaseDL<Voucher>, IVoucherDL
    {
        /// <summary>
        /// Lấy danh sách các chứng từ có chọn lọc
        /// </summary>
        /// <param name="keyword">Từ để tìm kiếm theo số chứng từ và nội dung</param>
        /// <param name="limit">Số chứng từ muốn lấy</param>
        /// <param name="page">Số trang bắt đầu lấy</param>
        /// <returns>Danh sách các chứng từ sau khi chọn lọc và các giá trị khác</returns>
        /// Created by: NNNINH (27/12/2022)
        public PagingData<Voucher> FilterVouchers(string? keyword, int limit, int page)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            parameters.Add("v_Offset", (page - 1) * limit);
            parameters.Add("v_Limit", limit);
            parameters.Add("v_Sort", "");

            var whereConditions = new List<string>();
            if (keyword != null) whereConditions.Add($"(voucher_code LIKE \'%{keyword}%\' OR description LIKE \'%{keyword}%\')");
            string whereClause = string.Join(" AND ", whereConditions);

            parameters.Add("v_Where", whereClause);

            // Khai báo tên prodecure
            string storedProcedureName = "Proc_voucher_GetPaging";

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            var filterResponse = new PagingData<Voucher>();
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Thực hiện gọi vào DB để chạy procedure
                var multiVouchers = mysqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Xử lý dữ liệu trả về
                var vouchers = multiVouchers.Read<Voucher>();
                var totalCount = multiVouchers.Read<long>().Single();
                var totalCost = multiVouchers.Read<long>().Single();

                filterResponse = new PagingData<Voucher>(vouchers, totalCount, 0, totalCost, 0, 0);
            }

            return filterResponse;
        }

        /// <summary>
        /// Lấy chi tiết chứng từ
        /// </summary>
        /// <param name="voucherId">Id chứng từ</param>
        /// <param name="limit">Số bản ghi muốn lấy</param>
        /// <param name="page">Số trang bắt đầu lấy</param>
        /// <returns>Danh sách các tài sản theo chứng từ</returns>
        /// Created by: NNNINH (01/01/2023)
        public PagingData<Asset> GetVoucherDetail(string? keyword, Guid voucherId, int limit, int page)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            parameters.Add("v_voucher_id", voucherId);
            parameters.Add("v_Offset", (page - 1) * limit);
            parameters.Add("v_Limit", limit);
            parameters.Add("v_Sort", "");

            var whereConditions = new List<string>();
            if (keyword != null) whereConditions.Add($"(fa.fixed_asset_code LIKE \'%{keyword}%\' OR fa.fixed_asset_name LIKE \'%{keyword}%\')");
            string whereClause = string.Join(" AND ", whereConditions);

            parameters.Add("v_Where", whereClause);

            // Khai báo tên prodecure
            string storedProcedureName = "Proc_voucher_GetDetail";

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
                var totalCost = multiAssets.Read<long>().Single();
                var totalDepreciation = multiAssets.Read<long>().Single();
                var totalRemain = multiAssets.Read<long>().Single();

                filterResponse = new PagingData<Asset>(assets, totalCount, 0, totalCost, totalDepreciation, totalRemain);
            }

            return filterResponse;
        }

        /// <summary>
        /// Thêm nhiều tài sản trong chứng từ
        /// </summary>
        /// <param name="voucherId">ID chứng từ đang sửa</param>
        /// <param name="assetIdList">Danh sách ID các tài sản cần thêm</param>
        /// <returns>Danh sách ID các tài sản đã thêm</returns>
        /// Cretaed by: NNNINH (06/01/2023)
        public int AddVoucherDetail(Guid voucherId, List<VoucherDetail> voucherDetails)
        {

            foreach (var items in voucherDetails)
            {
                items.voucher_detail_id = Guid.NewGuid();
                items.voucher_id = voucherId;
            }
            // Khởi tạo kết nối tới DB MySQL
            int numberOfAffectedRows = 0;
            string connectionString = DataContext.MySqlConnectionString;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure
                string storedProcedureName = "Proc_voucher_detail_BatchAdd";

                mysqlConnection.Open();

                // Bắt đầu transaction.
                using (var transaction = mysqlConnection.BeginTransaction())
                {
                    try
                    {

                        // Chuyển đổi list sang json


                        // Chuẩn bị tham số đầu vào
                        // Chuyển đổi list sang json
                        var listDatas = JsonSerializer.Serialize(voucherDetails);

                        // Chuẩn bị tham số đầu vào
                        var parameters = new DynamicParameters();
                        parameters.Add($"v_asset_detaill", listDatas);
                        numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                        if (numberOfAffectedRows == voucherDetails.Count)
                        {
                            transaction.Commit();

                            return numberOfAffectedRows;
                        }
                        else
                        {
                            transaction.Rollback();
                            numberOfAffectedRows = 0;
                            return numberOfAffectedRows;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        mysqlConnection.Close();
                        numberOfAffectedRows = 0;
                        return numberOfAffectedRows;
                    }
                    finally
                    {
                        mysqlConnection.Close();
                    }
                }
            }

        }

        /// <summary>
        /// Cập nhật nhiều tài sản trong chứng từ
        /// </summary>
        /// <param name="voucherId">ID chứng từ đang sửa</param>
        /// <param name="assetIdList">Danh sách ID các tài sản cần thêm</param>
        /// <returns>Danh sách ID các tài sản đã thêm</returns>
        /// Cretaed by: NNNINH (06/01/2023)
        public int UpadateVoucherDetail(Guid voucherId, List<Asset> assetDataill)
        {
            
            var listVoucherDetail = new List<Dictionary<string, object>>();

            foreach (var items in assetDataill)
            {
                if (items.flag != 0) {
                    var obj = new Dictionary<string, object>();
                    obj["voucher_detail_id"] = Guid.NewGuid();
                    obj["voucher_id"] = voucherId;
                    obj["fixed_asset_id"] = items.fixed_asset_id;
                    obj["budget"] = items.budget;
                    obj["oldCost"] = items.oldCost;
                    obj["created_by"] = Resource.DefaultUser;
                    obj["created_date"] = DateTime.Now;
                    obj["modified_by"] = Resource.DefaultUser;
                    obj["modified_date"] = DateTime.Now;
                    obj["flag"] = items.flag;
                    listVoucherDetail.Add(obj);
                }   
            }
            // Chuyển đổi list sang json
            var listDatas = JsonSerializer.Serialize(listVoucherDetail);
            var parameters = new DynamicParameters();
            parameters.Add($"v_asset_detaill", listDatas);
            // Khởi tạo kết nối tới DB MySQL
            int numberOfAffectedRows = 0;
            string connectionString = DataContext.MySqlConnectionString;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure
                string storedProcedureName = "Proc_voucher_detail_BatchUpdate";

                mysqlConnection.Open();

                // Bắt đầu transaction.
                using (var transaction = mysqlConnection.BeginTransaction())
                {
                    try
                    {
                       
                        numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                        if (numberOfAffectedRows > 0)
                        {
                            transaction.Commit();

                            return numberOfAffectedRows;
                        }
                        else
                        {
                            transaction.Rollback();
                            numberOfAffectedRows = 0;
                            return numberOfAffectedRows;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        mysqlConnection.Close();
                        numberOfAffectedRows = 0;
                        return numberOfAffectedRows;
                    }
                    finally
                    {
                        mysqlConnection.Close();
                    }
                }
            }

        }

        /// <summary>
        /// Lấy 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID của bản ghi cần lấy</param>
        /// <returns>Bản ghi có ID được truyền vào</returns>
        /// Created by:  NNNINH (10/11/2022)
        public VoucherDetail GetVoucherDetailById(Guid voucherId, Guid assetId)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(VoucherDetail).GetProperties();
            foreach (var property in properties)
            {
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {

                    parameters.Add($"v_fixed_asset_id", assetId);
                    break;
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            VoucherDetail record;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = "Proc_voucher_detail_Select";

                // Thực hiện gọi vào DB để chạy procedure
                record = mysqlConnection.QueryFirstOrDefault<VoucherDetail>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            return record;
        }

        /// <summary>
        /// Thêm 1 chứng từ kèm detaill
        /// </summary>
        /// <param name="recordId">ID của bản ghi</param>
        /// <returns>Bản ghi có ID được truyền vào</returns>
        /// Created by:  NNNINH (11/1/2023)
        public Guid BackAddVoucherDetail(BackAddVoucherDetaill backAddVoucherDetaill, Guid recordId)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(Voucher).GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue;
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    propertyValue = recordId;
                }
                else
                {
                    propertyValue = property.GetValue(backAddVoucherDetaill.Voucher, null);
                }
                parameters.Add($"v_{propertyName}", propertyValue);
            }
            var voucherData = new List<List<VoucherDetail>>();
            var listVoucherDetail = new List<Dictionary<string, object>>();

            foreach (var items in backAddVoucherDetaill.listAssetDetail)
            {
                var obj = new Dictionary<string, object>();
                obj["voucher_detail_id"] = Guid.NewGuid();
                obj["voucher_id"] = recordId;
                obj["fixed_asset_id"] = items.fixed_asset_id;
                obj["budget"] = items.budget;
                obj["oldCost"] = items.oldCost;
                obj["created_by"] = Resource.DefaultUser;
                obj["created_date"] = DateTime.Now;
                obj["modified_by"] = Resource.DefaultUser;
                obj["modified_date"] = DateTime.Now;
                obj["flag"] = items.flag;
                listVoucherDetail.Add(obj);
            }
            var listDatas = JsonSerializer.Serialize(listVoucherDetail);
            parameters.Add($"v_voucher_detaill", listDatas);
            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;

            int numberOfAffectedRows = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure
                string storedProcedureName = "Proc_voucher_Add";

                mysqlConnection.Open();

                // Bắt đầu transaction.
                using (var transaction = mysqlConnection.BeginTransaction())
                {
                    try
                    {
                        numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                        if (numberOfAffectedRows == backAddVoucherDetaill.listAssetDetail.Count)
                        {
                            transaction.Commit();

                            return recordId;
                        }
                        else
                        {
                            transaction.Rollback();
                            numberOfAffectedRows = 0;
                            return Guid.Empty;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        mysqlConnection.Close();
                        numberOfAffectedRows = 0;
                        return Guid.Empty;
                    }
                    finally
                    {
                        mysqlConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Kiểm tra trùng mã bản ghi
        /// </summary>
        /// <param name="recordCode">Mã cần xét trùng</param>
        /// <param name="recordId">Id bản ghi đưa vào (nếu là sửa)</param>
        /// <returns>Số lượng mã tài sản bị trùng</returns>
        /// Cretaed by:  NNNINH (09/11/2022)
        public int DuplicateVoucherCode(object recordCode, Guid recordId)
        {
            // Khai báo tên prodecure
            string storedProcedureName = "Proc_voucher_DuplicateCode";

            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();

            if (recordCode != null && recordId != null)
            {
                parameters.Add($"v_voucher_code", recordCode);
                parameters.Add($"v_voucher_id", recordId);
            }
            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int duplicates = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                duplicates = mysqlConnection.QueryFirstOrDefault<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }

            return duplicates;
        }
    }
}
