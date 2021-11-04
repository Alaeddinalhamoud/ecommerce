using ecommerce.Data;
using ecommerce.DataAccess;
using ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Repositories
{
    public class CartLineRepository : Repository<CartLine>, ICartLine
    {
        private readonly ApplicationDbContext _db;
        public CartLineRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<POJO> Save(CartLine entity)
        {
            POJO model = new POJO();
            if (entity.id == 0)
            {
                try
                {
                    //check if already is added, to update the qty and price only.
                    var oldCartLine = _db.CartLines
                        .FirstOrDefault(x => x.productId.Equals(entity.productId) && x.createdBy.Equals(entity.createdBy) && x.cartId.Equals(entity.cartId));
                    if (oldCartLine == null)
                    {
                        //Add the total per line
                        entity.total = entity.price * entity.qty;
                        await _db.CartLines.AddAsync(entity);
                        await _db.SaveChangesAsync();
                        model.id = entity.id.ToString();
                        model.flag = true;
                        model.message = "Has Been Added.";
                    }
                    else
                    {
                       //If the product already exsit just update the qty
                        model = await UpdateCartLine(oldCartLine, entity);
                    }
                }
                catch (Exception ex)
                {
                    model.flag = false;
                    model.message = ex.ToString();
                }
            }
            else if (entity.id != 0)
            {
                CartLine _Entity = await _db.CartLines.FindAsync(entity.id);
                model = await UpdateCartLine(_Entity, entity);
            }
            return model;
        } 

        //partial update
        async Task<POJO> UpdateCartLine(CartLine _Entity, CartLine entity)
        {
            POJO model = new POJO();
           // _Entity.id = entity.id;
            _Entity.cartId = entity.cartId.Equals(0) ? _Entity.cartId : entity.cartId;
            _Entity.productId = entity.productId.Equals(0) ? _Entity.productId : entity.productId;
            _Entity.qty = _Entity.qty + entity.qty;
            _Entity.freeTax = entity.freeTax;
            _Entity.freeShipping = entity.freeShipping;
            _Entity.price = entity.price.Equals(0) ? _Entity.price : entity.price;
            //This total is price per item * qty
            //QTY = -1 mean the user has pressed on - in the cart, we have to calc the total in the cart again.
            if (entity.qty.Equals(-1))
            {
                _Entity.total = _Entity.total - entity.price;
            }
            else
            {
                _Entity.total = entity.price * _Entity.qty;
            }
            // _Entity.createDate = entity.createDate;
            _Entity.updateDate = DateTime.Now;
            _Entity.modifiedBy = entity.modifiedBy;
            try
            {
                await _db.SaveChangesAsync();
                //We are returning Qty to show it in Modify Product Qty in the cart
                model.id = _Entity.qty.ToString();
                model.flag = true;
                model.message = "Has Been Updated.";
            }
            catch (Exception ex)
            {
                model.flag = false;
                model.message = ex.ToString();
            }
            return model;
        }
    }
}
