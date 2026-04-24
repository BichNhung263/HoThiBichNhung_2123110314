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
            string vnp_Returnurl = "http://localhost:5173/vnpay-return"; // URL frontend nhận kết quả
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // URL sandbox VNPay
            string vnp_TmnCode = "2QXUI4J4"; // Mã website (TMN Code) Demo chuẩn
            string vnp_HashSecret = "GET8897A8V6UE0GSS7648ENY90M0E3X6"; // Chuỗi bí mật Demo chuẩn

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(request.Amount * 100)).ToString()); // Số tiền nhân 100
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1"); // Ép về IPv4 để tránh lỗi chữ ký
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang " + request.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); // default value
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", request.OrderId.ToString()); // Mã tham chiếu của giao dịch (Order ID)

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return Ok(new { url = paymentUrl });
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

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
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
                        var order = await _context.Orders
                            .Include(o => o.OrderDetails)
                            .FirstOrDefaultAsync(o => o.Id == orderId);
                            
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
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
