using System;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Generalized object storing basic information on orders and their status
    /// </summary>
    [Umbraco.Core.Persistence.TableName("customNetPaymentOrder")]
    [Umbraco.Core.Persistence.PrimaryKey("Id", autoIncrement = false)]
    public class OrderStatus
    {
        /// <summary>
        /// Description of ordered item or items f.x.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Order SQL unique Id
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Friendly order name: f.x. IS0001
        /// </summary>
        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(50)]
        public string OrderName { get; set; }

        /// <summary>
        /// Umbraco member id
        /// </summary>
        public int Member { get; set; }

        /// <summary>
        /// Total amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Length(45)]
        public string IPAddress { get; set; }

        /// <summary>
        /// Browser User agent
        /// </summary>
        [Length(255)]
        public string UserAgent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Paid { get; set; }

        /// <summary>
        /// String name of payment provider <see cref="IPublishedContent"/> node
        /// Helps to resolve overloaded payment providers, f.x. Borgun USD and Borgun ISK
        /// </summary>
        [Length(50)]
        public string PaymentProvider { get; set; }

        /// <summary>
        /// Store custom order data here
        /// </summary>
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Custom { get; set; }

        /// <summary>
        /// Store custom order data here
        /// </summary>
        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(200)]
        public string NetPaymentData { get; set; }
    }
}
