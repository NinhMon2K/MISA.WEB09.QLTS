using MISA.WEB09.QLTS.Common.Entities;
using MISA.WEB09.QLTS.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB09.QLTS.BL
{
    public class AssetBL : BaseBL<Asset>, IAssetBL
    {
        #region Field

        private IAssetDL _assetDL;

        #endregion

        #region Constructer

        public AssetBL(IAssetDL assetDL) : base(assetDL)
        {
            _assetDL = assetDL;
        }

        #endregion

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
            return _assetDL.FilterAssets(pagingAsset);
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
            return _assetDL.NextAssetCode();
        } 
        #endregion
    }
}
