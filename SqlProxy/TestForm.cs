using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SqlProxy
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        public class MyClass
        {
            [DisplayName("Номер заказа")]
            public int OrderId { get; set; }

            [DisplayName("Номер заказчика")]
            public int CustomerId { get; set; }

            [DisplayName("Наименованеи заказчика")]
            public string CustomerName { get; set; }
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            var db = new SqlProxyDC();
            int id = 1;
            var orders = db.Orders.Where(o => o.Id == id);

            var xmlRes =
                orders.Select(
                    o => new MyClass {OrderId = o.Id, CustomerId = o.CustomerId, CustomerName = o.Customer.Name});

            var result = ProxyQuery(db, xmlRes);

            grid.DataSource = result.ToList();
        }

        static DataTable ReadData(string connection, string sql, DbParameterCollection sqlParams)
        {
            using (var conn = new SqlConnection())
            {
                conn.ConnectionString = connection;
                conn.Open();
                using (var query = conn.CreateCommand())
                {
                    query.CommandText = sql;
                    query.CommandType = CommandType.Text;
                    for (int index = 0; index < sqlParams.Count; index++)
                    {
                        var parameter = sqlParams[index];
                        query.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                    }
                    using (var reader = query.ExecuteReader())
                    {
                        var res = new DataTable();
                        res.Load(reader);
                        return res;
                    }
                }
            }
        }

        private static IEnumerable<T> ProxyQuery<T>(SqlProxyDC db, IQueryable<T> xmlRes)
            where T : new()
        {
            var type = typeof (T);
            var prop = type.GetProperties();
            var data = ReadData(
                @"Data Source=(localdb)\ProjectsV12;Initial Catalog=SqlProxyDB;Integrated Security=True;Connect Timeout=30",
                xmlRes.ToString(),
                db.GetCommand(xmlRes).Parameters);
            foreach (var row in data.Rows.OfType<DataRow>())
            {
                var res = new T();
                foreach (var propertyInfo in prop)
                {
                    propertyInfo.SetValue(res, row[propertyInfo.Name]);
                }
                yield return res;
            }
        }
    }
}
