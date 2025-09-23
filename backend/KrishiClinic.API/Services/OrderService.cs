using Microsoft.EntityFrameworkCore;
using KrishiClinic.API.Data;
using KrishiClinic.API.Models;
using KrishiClinic.API.DTOs;

namespace KrishiClinic.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly KrishiClinicDbContext _context;

        public OrderService(KrishiClinicDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Convert to DTOs to avoid circular references
            return orders.Select(o => new
            {
                orderId = o.OrderId,
                userId = o.UserId,
                orderNumber = o.OrderNumber,
                totalAmount = o.TotalAmount,
                status = o.Status,
                paymentMethod = o.PaymentMethod,
                deliveryAddress = o.DeliveryAddress,
                deliveryVillage = o.DeliveryVillage,
                orderDate = o.OrderDate,
                shippedDate = o.ShippedDate,
                deliveredDate = o.DeliveredDate,
                createdAt = o.CreatedAt,
                updatedAt = o.UpdatedAt,
                orderItems = o.OrderItems.Select(oi => new
                {
                    orderItemId = oi.OrderItemId,
                    productId = oi.ProductId,
                    productName = oi.ProductName,
                    quantity = oi.Quantity,
                    unitPrice = oi.UnitPrice,
                    totalPrice = oi.TotalPrice,
                    productImageUrl = oi.ProductImageUrl,
                    product = oi.Product != null ? new
                    {
                        productId = oi.Product.ProductId,
                        name = oi.Product.Name,
                        description = oi.Product.Description,
                        price = oi.Product.Price,
                        imageUrl = oi.Product.ImageUrl,
                        category = oi.Product.Category
                    } : null
                })
            });
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<object> CreateOrderAsync(CreateOrderDto orderDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generate unique order number
                var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

                var order = new Order
                {
                    UserId = orderDto.UserId,
                    OrderNumber = orderNumber,
                    PaymentMethod = orderDto.PaymentMethod,
                    DeliveryAddress = orderDto.DeliveryAddress,
                    DeliveryVillage = orderDto.DeliveryVillage,
                    Status = "Processing",
                    OrderDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                decimal totalAmount = 0;
                var orderItems = new List<object>();

                foreach (var item in orderDto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null)
                        throw new ArgumentException($"Product with ID {item.ProductId} not found");

                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = item.Quantity,
                        TotalPrice = product.Price * item.Quantity,
                        ProductImageUrl = product.ImageUrl,
                        CreatedAt = DateTime.UtcNow
                    };

                    totalAmount += orderItem.TotalPrice;
                    _context.OrderItems.Add(orderItem);

                    // Add to orderItems list for response
                    orderItems.Add(new
                    {
                        orderItemId = orderItem.OrderItemId,
                        productId = orderItem.ProductId,
                        productName = orderItem.ProductName,
                        quantity = orderItem.Quantity,
                        unitPrice = orderItem.UnitPrice,
                        totalPrice = orderItem.TotalPrice,
                        productImageUrl = orderItem.ProductImageUrl
                    });
                }

                order.TotalAmount = totalAmount;
                await _context.SaveChangesAsync();

                // Clear user's cart after successful order
                var cartItems = await _context.Carts.Where(c => c.UserId == orderDto.UserId).ToListAsync();
                _context.Carts.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return DTO to avoid circular references
                return new
                {
                    orderId = order.OrderId,
                    userId = order.UserId,
                    orderNumber = order.OrderNumber,
                    totalAmount = order.TotalAmount,
                    status = order.Status,
                    paymentMethod = order.PaymentMethod,
                    deliveryAddress = order.DeliveryAddress,
                    deliveryVillage = order.DeliveryVillage,
                    orderDate = order.OrderDate,
                    shippedDate = order.ShippedDate,
                    deliveredDate = order.DeliveredDate,
                    createdAt = order.CreatedAt,
                    updatedAt = order.UpdatedAt,
                    orderItems = orderItems
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto statusDto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            order.Status = statusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            // Update relevant dates based on status
            switch (statusDto.Status.ToLower())
            {
                case "shipped":
                    order.ShippedDate = DateTime.UtcNow;
                    break;
                case "delivered":
                    order.DeliveredDate = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<object>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Convert to DTOs to avoid circular references
            return orders.Select(o => new
            {
                orderId = o.OrderId,
                userId = o.UserId,
                orderNumber = o.OrderNumber,
                totalAmount = o.TotalAmount,
                status = o.Status,
                paymentMethod = o.PaymentMethod,
                deliveryAddress = o.DeliveryAddress,
                deliveryVillage = o.DeliveryVillage,
                orderDate = o.OrderDate,
                shippedDate = o.ShippedDate,
                deliveredDate = o.DeliveredDate,
                createdAt = o.CreatedAt,
                updatedAt = o.UpdatedAt,
                user = new
                {
                    userId = o.User.UserId,
                    name = o.User.Name,
                    mobile = o.User.Mobile,
                    village = o.User.Village,
                    address = o.User.Address,
                    isActive = o.User.IsActive,
                    createdAt = o.User.CreatedAt
                },
                orderItems = o.OrderItems.Select(oi => new
                {
                    orderItemId = oi.OrderItemId,
                    productId = oi.ProductId,
                    productName = oi.ProductName,
                    quantity = oi.Quantity,
                    unitPrice = oi.UnitPrice,
                    totalPrice = oi.TotalPrice,
                    productImageUrl = oi.ProductImageUrl,
                    product = oi.Product != null ? new
                    {
                        productId = oi.Product.ProductId,
                        name = oi.Product.Name,
                        description = oi.Product.Description,
                        price = oi.Product.Price,
                        imageUrl = oi.Product.ImageUrl,
                        category = oi.Product.Category
                    } : null
                })
            });
        }

        public async Task<decimal> GetOrderTotalAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .SumAsync(oi => oi.TotalPrice);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, DateTime? shippedDate, DateTime? deliveredDate)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            if (shippedDate.HasValue)
                order.ShippedDate = shippedDate.Value;
            if (deliveredDate.HasValue)
                order.DeliveredDate = deliveredDate.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetOrderCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == "Delivered")
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetPendingOrderCountAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == "Processing")
                .CountAsync();
        }

        public async Task<int> GetOrderCountByStatusAsync(string status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .CountAsync();
        }

        public async Task<IEnumerable<object>> GetRecentOrdersAsync(int count)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();

            // Convert to DTOs to avoid circular references
            return orders.Select(o => new
            {
                orderId = o.OrderId,
                userId = o.UserId,
                orderNumber = o.OrderNumber,
                totalAmount = o.TotalAmount,
                status = o.Status,
                paymentMethod = o.PaymentMethod,
                deliveryAddress = o.DeliveryAddress,
                deliveryVillage = o.DeliveryVillage,
                orderDate = o.OrderDate,
                shippedDate = o.ShippedDate,
                deliveredDate = o.DeliveredDate,
                createdAt = o.CreatedAt,
                updatedAt = o.UpdatedAt,
                user = new
                {
                    userId = o.User.UserId,
                    name = o.User.Name,
                    mobile = o.User.Mobile,
                    village = o.User.Village,
                    address = o.User.Address,
                    isActive = o.User.IsActive,
                    createdAt = o.User.CreatedAt
                },
                orderItems = o.OrderItems.Select(oi => new
                {
                    orderItemId = oi.OrderItemId,
                    productId = oi.ProductId,
                    productName = oi.ProductName,
                    quantity = oi.Quantity,
                    unitPrice = oi.UnitPrice,
                    totalPrice = oi.TotalPrice,
                    productImageUrl = oi.ProductImageUrl,
                    product = oi.Product != null ? new
                    {
                        productId = oi.Product.ProductId,
                        name = oi.Product.Name,
                        description = oi.Product.Description,
                        price = oi.Product.Price,
                        imageUrl = oi.Product.ImageUrl,
                        category = oi.Product.Category
                    } : null
                })
            });
        }
    }
}
