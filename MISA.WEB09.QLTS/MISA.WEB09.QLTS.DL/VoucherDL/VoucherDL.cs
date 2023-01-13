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
        /// Thêm mới chứng từ kèm detail
        /// <param name="voucherId">ID chứng từ đang thêm</param>
        /// <param name="backAddVoucherDetaill">Đối tượng chứng từ kèm danh sách tài sản cần chứng từ</param>
        /// <returns>Mã chứng từ vừa thêm</returns>
        /// Cretaed by: NNNINH (06/01/2023)
        public Guid BackAddVoucherDetail(BackAddVoucherDetaill backAddVoucherDetaill, Guid recordId)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            DynamicParameters parameters = PramatersProperties(recordId, backAddVoucherDetaill.Voucher);

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
        /// Cập nhật chứng từ kèm detail 
        /// </summary>
        /// <param name="voucherId">ID chứng từ đang sửa</param>
        /// <param name="backAddVoucherDetaill">Đối tượng chứng từ kèm danh sách tài sản cần chứng từ</param>
        /// <returns>Mã chứng từ vừa sửa</returns>
        /// Cretaed by: NNNINH (06/01/2023)
        public Guid UpadateVoucherDetail(Guid voucherId, BackAddVoucherDetaill backAddVoucherDetaill)
        {
            DynamicParameters parameters = PramatersProperties(voucherId, backAddVoucherDetaill.Voucher);
            var listVoucherDetail = new List<Dictionary<string, object>>();

            foreach (var items in backAddVoucherDetaill.listAssetDetail)
            {
                if (items.flag != 0)
                {
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

            parameters.Add($"v_asset_detaill", listDatas);
            // Khởi tạo kết nối tới DB MySQL
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

                        mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);
                        transaction.Commit();
                        return voucherId;

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        mysqlConnection.Close();
                        return Guid.Empty;
                    }
                    finally
                    {
                        mysqlConnection.Close();
                    }
                }
            }

        }


    }
}
