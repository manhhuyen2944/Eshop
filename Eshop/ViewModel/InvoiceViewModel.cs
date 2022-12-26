using Eshop.Models;

namespace Eshop.ViewModel
{
    public class InvoiceViewModel
    {
        public Invoice Invoices { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
