using HoThiBichNhung_2123110314.Data;
using HoThiBichNhung_2123110314.Models;
using HoThiBichNhung_2123110314.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HoThiBichNhung_2123110314.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> Get()
            => await _context.Payments.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Payment>> Post(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return Ok(payment);
        }

        // 🔥 Tạo URL thanh toán VNPay
        [HttpPost("create-vnpay-url")]
        public IActionResult CreateVnPayUrl([FromBody] VnPayRequest request)
        {
            // Thay vì dùng VNPay thật, ta dùng VNPay giả để Demo mượt mà
            string mockPaymentUrl = $"http://localhost:5173/mock-payment?orderId={request.OrderId}&amount={request.Amount}";
            
            // Nếu bạn đang chạy trên Render, hãy dùng link Render của Frontend
            // string mockPaymentUrl = $"https://nhung-frontend.onrender.com/mock-payment?orderId={request.OrderId}&amount={request.Amount}";

            return Ok(new { url = mockPaymentUrl });
        }

        // 🔥 Callback xác nhận kết quả từ VNPay (IPN)
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            if (Request.Query.Count > 0)
            {
                string vnp_HashSecret = "GET8897A8V6UE0GSS7648ENY90M0E3X6";
                var vnpayData = Request.Query;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData.Keys)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }

                string orderId = vnpay.GetResponseData("vnp_TxnRef");
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.Query["vnp_SecureHash"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        // Thanh toán thành công -> Cập nhật đơn hàng
                        // Lưu ý: Nếu orderId của bạn là string, hãy đảm bảo bảng Orders dùng string hoặc parse ngược lại nếu là số
                        var order = await _context.Orders
                            .Include(o => o.OrderDetails)
                            .FirstOrDefaultAsync(o => o.Id.ToString() == orderId);
                            
                        if (order != null)
                        {
                            order.Status = OrderStatus.Paid; 
                            
                            // Trừ số lượng tồn kho
                            if (order.OrderDetails != null)
                            {
                                foreach (var detail in order.OrderDetails)
                                {
                                    var product = await _context.Products.FindAsync(detail.ProductId);
                                    if (product != null)
                                    {
                                        product.Quantity -= detail.Quantity;
                                        if (product.Quantity < 0) product.Quantity = 0;
                                    }
                                }
                            }
                            
                            await _context.SaveChangesAsync();
                        }
                        return Ok(new { message = "Thanh toán thành công", orderId = orderId });
                    }
                    else
                    {
                        return BadRequest(new { message = "Thanh toán thất bại", code = vnp_ResponseCode });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Chữ ký không hợp lệ" });
                }
            }
            return BadRequest();
        }
    }

    public class VnPayRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
