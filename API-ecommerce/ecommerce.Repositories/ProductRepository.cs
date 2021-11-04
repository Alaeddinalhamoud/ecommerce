using ecommerce.Data;
using ecommerce.Data.MVData;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
   public class ProductRepository : Repository<Product>, IProduct
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public async Task<IQueryable<ProductDetail>> ProductDetail()
        //{
        //   IQueryable<ProductDetail> productDetail = null;
            
        //        try 
        //        {
        //         productDetail = from product in _db.Products
        //                    join brand in _db.Brands on product.brandId equals brand.id
        //                    join category in _db.Categories on product.categoryId equals category.id                            
        //                    join productType in _db.ProductType on product.productTypeId equals productType.id                                 
        //                    select new  ProductDetail
        //                    {
        //                         id = product.id,
        //                         name = product.name,
        //                         createdBy = product.createdBy,
        //                         vendorId = product.vendorId,
        //                         isDeleted = product.isDeleted,
        //                         price = product.price,
        //                         categoryId = product.categoryId,
        //                         description = product.description,
        //                         tags = product.tags,
        //                         isApproved = product.isApproved,
        //                         videoUrl = product.videoUrl,
        //                         rating = product.rating,
        //                         placeOfOrigin = product.placeOfOrigin,
        //                         modelNumber = product.modelNumber,
        //                         expiryDate = product.expiryDate,
        //                         barcode = product.barcode,
        //                         inventoryCode = product.inventoryCode,
        //                         LotNumber = product.LotNumber,
        //                         brandId = product.brandId,
        //                         productTypeId = product.productTypeId,
        //                         createDate = product.createDate,
        //                         updateDate = product.updateDate,
        //                         modifiedBy = product.modifiedBy,
        //                         brandName = brand.name,
        //                         categoryName = category.name,
        //                         medias = _db.Medias,
        //                         productSpecifications = _db.ProductSpecification,
        //                         productTypeName = productType.name,
        //                         reviews = _db.Reviews                                 
        //                    }; 
        //        }
        //        catch(Exception ex)
        //        {

        //        }           
        //    return productDetail;
        //}

        public async Task<POJO> Save(Product entity)
        {
            POJO model = new POJO();
            try
            {
                if(!String.IsNullOrEmpty(entity.barcode))
                {
                    var isBarCode = _db.Products.Where(x => x.barcode.Equals(entity.barcode) && !(x.id.Equals(entity.id))).Any();
                    if (isBarCode)
                    {
                        model.flag = false;
                        model.message = $"{entity.barcode} BarCode Already Exist.";
                        return model;
                    }
                } 
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = ex.ToString();
            }

            if (entity.id == 0)
            {
                try
                {
                    await _db.Products.AddAsync(entity);
                    await _db.SaveChangesAsync();
                    model.id = entity.id.ToString();
                    model.flag = true;
                    model.message = "Has Been Added.";
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }           
            else if (entity.id != 0)
            {
                Product _Entity = await _db.Products.FindAsync(entity.id);
                _Entity.id = entity.id;
                _Entity.name = String.IsNullOrEmpty(entity.name) ? _Entity.name : entity.name;
                //  _Entity.userId = entity.userId;
                //  _Entity.vendorId = entity.vendorId; // no need to update th vendor ID (owner)
                // _Entity.isDeleted = entity.isDeleted;
                _Entity.oldPrice = entity.oldPrice;
                _Entity.price = entity.price.Equals(0) ? _Entity.price : entity.price;
              //  _Entity.userId = entity.userId;
                _Entity.categoryId = entity.categoryId == 0  ? _Entity.categoryId : entity.categoryId;
                _Entity.description = String.IsNullOrEmpty(entity.description) ? _Entity.description : entity.description;
                _Entity.tags = String.IsNullOrEmpty(entity.tags) ? _Entity.tags : entity.tags;
                _Entity.isApproved = entity.isApproved;
                _Entity.freeShipping = entity.freeShipping;
                _Entity.freeTax = entity.freeTax;
                _Entity.videoUrl = String.IsNullOrEmpty(entity.videoUrl) ? _Entity.videoUrl : entity.videoUrl;
                _Entity.rating = entity.rating.Equals(0) ? _Entity.rating : entity.rating;
                _Entity.placeOfOrigin = entity.placeOfOrigin == 0 ? _Entity.placeOfOrigin : entity.placeOfOrigin;
                _Entity.modelNumber = String.IsNullOrEmpty(entity.modelNumber) ? _Entity.modelNumber : entity.modelNumber;
                _Entity.expiryDate = entity.expiryDate.Equals("0001-01-01T00:00:00") ? _Entity.expiryDate : entity.expiryDate;
                _Entity.barcode = String.IsNullOrEmpty(entity.barcode) ? _Entity.barcode : entity.barcode;
                _Entity.inventoryCode = String.IsNullOrEmpty(entity.inventoryCode) ? _Entity.inventoryCode : entity.inventoryCode;
                _Entity.LotNumber = String.IsNullOrEmpty(entity.LotNumber) ? _Entity.LotNumber : entity.LotNumber;
                _Entity.brandId = entity.brandId == 0 ? _Entity.brandId : entity.brandId;
                _Entity.productTypeId = entity.productTypeId == 0 ? _Entity.productTypeId : entity.productTypeId;
                // _Entity.createDate = entity.createDate;
                _Entity.updateDate = DateTime.Now;
                _Entity.modifiedBy = String.IsNullOrEmpty(entity.modifiedBy) ?_Entity.modifiedBy : entity.modifiedBy;
              //  _Entity.frequency = entity.frequency.Equals(0) ? _Entity.frequency : entity.frequency; We have created new TB called Most Viewed Product.
                _Entity.qty = entity.qty;
                _Entity.packageType = entity.packageType == 0 ? _Entity.packageType : entity.packageType;
                _Entity.shortDescription = String.IsNullOrEmpty(entity.shortDescription) ? _Entity.shortDescription : entity.shortDescription;
                try
                {
                    await _db.SaveChangesAsync();
                    model.id = _Entity.id.ToString();
                    model.flag = true;
                    model.message = "Has Been Updated.";
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            return model;
        }
        public async Task<POJO> UpdateIsDelete(Product entity)
        {
            POJO model = new POJO();
            if (entity.id != 0)
            {
                Product _Entity = await _db.Products.FindAsync(entity.id);
                _Entity.isDeleted = entity.isDeleted;
                _Entity.updateDate = entity.updateDate;
                _Entity.modifiedBy = entity.modifiedBy;
                try
                {
                    await _db.SaveChangesAsync();
                    model.id = _Entity.id.ToString();
                    model.flag = true;
                    model.message = "Has Been Updated.";
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            return model;
        }
    }
}
