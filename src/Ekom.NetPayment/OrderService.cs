using Microsoft.Practices.Unity;
using NPoco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core;

namespace Umbraco.NetPayment
{
	/// <summary>
	/// Utility functions for handling <see cref="OrderStatus"/> objects
	/// </summary>
	class OrderService
	{
		/// <summary>
		/// Attempts to retrieve an order using data from the querystring or posted values
		/// </summary>
		/// <returns>Returns the referenced order or null otherwise</returns>
		public static OrderStatus Get()
		{
			var container = UnityConfig.GetConfiguredContainer();
			var orderSvc = container.Resolve<OrderService>();

			var request = HttpContext.Current.Request;

			string reference = request.QueryString["ReferenceNumber"];

			if (string.IsNullOrEmpty(reference))
			{
				reference = request.QueryString["orderId"];
			}

			if (string.IsNullOrEmpty(reference))
			{
				reference = request["reference"];
			}

			if (string.IsNullOrEmpty(reference))
			{
				reference = request["orderid"];
			}

			if (!string.IsNullOrEmpty(reference))
			{
				bool _referenceId = Guid.TryParse(reference, out var referenceId);

				if (_referenceId)
				{
					return orderSvc.GetAsync(referenceId).Result;
				}
			}

			return null;
		}

		ApplicationContext _appCtx;
		Settings _settings;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="appCtx"></param>
		/// <param name="settings"></param>
		public OrderService(ApplicationContext appCtx, Settings settings)
		{
			_appCtx = appCtx;
			_settings = settings;
		}


		/// <summary>
		/// Get order with the given unique id
		/// </summary>
		/// <param name="id">Order id</param>
		public async Task<OrderStatus> GetAsync(Guid id)
		{
			using (var db = new Database(_settings.ConnectionStringName))
			{
				return await db.SingleByIdAsync<OrderStatus>(id).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Persist in database and retrieve unique order id
		/// </summary>
		/// <returns>Order Id</returns>
		public async virtual Task<Guid> SaveAsync(
			int member,
			string total,
			string paymentProvider,
			string custom,
			IEnumerable<OrderItem> orders
		)
		{
			NumberFormatInfo nfi = new CultureInfo("is-IS", false).NumberFormat;

			var name = new StringBuilder();

			foreach (var order in orders)
			{
				name.Append(order.Title + " ");
			}

			var orderid = Guid.NewGuid();

			using (var db = new Database(_settings.ConnectionStringName))
			{
				// Return order id
				await db.InsertAsync(new OrderStatus
				{
					Id = orderid,
					Name = name.ToString(),
					Member = member,
					Amount = decimal.Parse(total, nfi),
					Date = DateTime.Now,
					PaymentProvider = paymentProvider,
					Custom = custom
				}).ConfigureAwait(false);
			}

			return orderid;
		}
	}
}
