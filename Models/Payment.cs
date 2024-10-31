﻿namespace KhielsSkincare.Models
{
    public class Payment
    {
        public int Id { get; set; } // ID duy nhất của thanh toán
        public string PaymentMethod { get; set; } // Hình thức thanh toán (Tiền mặt, Chuyển khoản, Thẻ tín dụng, Ví điện tử)
        public decimal Amount { get; set; } // Số tiền thanh toán
        public DateTime PaymentDate { get; set; } // Ngày thanh toán
        public string TransactionId { get; set; } // ID giao dịch (nếu có)
        public string Status { get; set; } // Trạng thái thanh toán (thành công, thất bại, chờ xử lý, ...)
        public string OrderCode { get; internal set; }
    }
}
