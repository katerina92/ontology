using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class postalAddress
    {
        public String Prefix { get; set; }
        public string Country { get { return "Россия"; } }
        public string Region { get; set; }
        public String Locality { get; set; }
        public String Address { get; set; }
        public String PostalCode { get; set; }

        private String[] phone;
        public String Phone
        {
            get
            {
                if (phone == null || phone.Length < 1)
                    return String.Empty;
                StringBuilder res = new StringBuilder(phone[0]);
                for (int i = 1; i < phone.Length; i++)
                    res.Append(",").Append(phone[i]);
                return res.ToString();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    phone = new String[0];
                else
                    phone = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            }
        }

        public postalAddress(String prefix, String index, String data)
        {
            if (!String.IsNullOrEmpty(prefix))
                this.Prefix = prefix + ":";
            this.PostalCode = index;
            String v;
            data = GetData(data, "область", out v);
            if (v == null)
                data = GetData(data, "республика", out v);
            if (v == null)
                data = GetData(data, "край", out v);
            Region = v;

            data = GetData(data, "район", out v);
            Locality = v;
            Address = data;

        }

        public String GetData(String data, String field, out String value)
        {
            int n = data.ToLowerInvariant().IndexOf(" " + field.ToLowerInvariant() + " ");
            if (n < 0)
            {
                n = data.ToLowerInvariant().IndexOf(field.ToLowerInvariant() + " ");
                if (n != 0)
                {
                    value = null;
                    return data;
                }
            }
            value = data.Substring(0, n + field.Length + 1).Trim();
            return data.Substring(n + field.Length + 1).Trim();
        }

        public String GetString(String propertyName, String value)
        {
            return new StringBuilder().Append(Prefix).Append(propertyName).Append(' ').Append('\"').Append(value).Append("\";").ToString();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder("[").AppendLine();
            if (!String.IsNullOrEmpty(PostalCode))
                result.AppendLine(GetString("postalCode", PostalCode));
            if (!String.IsNullOrEmpty(Region))
                result.AppendLine(GetString("addressRegion", Region));
            if (!String.IsNullOrEmpty(Locality))
                result.AppendLine(GetString("addressLocality", Locality));
            if (!String.IsNullOrEmpty(Address))
                result.AppendLine(GetString("streetAddress", Address));
            if (!String.IsNullOrEmpty(Phone))
            {
                for (int i = 0; i < phone.Length; i++)
                    result.AppendLine(GetString("telephone", phone[i]));
            }

            result.Append(Prefix).Append("addressCountry").Append(' ').Append('\"').Append(Country).AppendLine("\"").Append(']');
            return result.ToString();

        }
    }
    class Program
    {
        const int LEN = 6;
        static void Main(string[] args)
        {
            using (SqlConnection cn = new SqlConnection(
                new SqlConnectionStringBuilder
                {
                    DataSource = "(local)",
                    IntegratedSecurity = true,
                    InitialCatalog = "ska2014_autumn"
                }.ConnectionString
                ))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = "SELECT top 5 zipcode, name, address, phone" +
                                    "  FROM d_zipcode_info(nolock)" +
                                    "  /*WHERE address like '%область%'*/"+
                                 " ORDER BY zipcode";
                    using (var dr = cmd.ExecuteReader())
                    {
                        using (var sr = new StreamWriter(File.Open("C:\\Users\\Ivan V. Kaurov\\Desktop\\Gmail\\test_small.ttl", FileMode.Create), Encoding.UTF8))
                        {
                            sr.Write(@"
@prefix post: <http://localhost:8080/resource/index/>.
@prefix ps: <http://schema.org/PostOffice/>.
@prefix adr: <http://www.schema.org/PostalAddress/>.
");

                            while (dr.Read())
                            {
                                sr.Write("{0}post:{1}{0}  ps:name \"{2}\";{0}  ps:address {3}.",
                                    Environment.NewLine, dr["zipcode"], dr["name"], new postalAddress("    " + "adr", dr["zipcode"].ToString(), dr["address"].ToString())
                                    {
                                        Phone = dr["phone"].ToString()
                                    });
                            }
                        }
                    }
                }
            }
            /*
            using (var sr = new StreamReader("C:\\Users\\Ivan V. Kaurov\\Desktop\\Gmail\\zipcode_info_data.sql"))
            {
                String sPrev = null, sCurrent;
                bool startFound = false;
                char[] buffer = new char[1024];
                int n, k, cnt = 0;
                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
                sb.DataSource = "(local)";
                sb.InitialCatalog = "ska2014_autumn";
                sb.IntegratedSecurity = true;
                using (SqlConnection cn = new SqlConnection(sb.ConnectionString))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = cn;
                        cmd.Transaction = cn.BeginTransaction();
                        try
                        {
                            cmd.CommandText = "delete from d_zipcode_info";
                            cmd.ExecuteNonQuery();
                            while ((n = sr.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                sCurrent = new string(buffer);
                                if (sPrev != null)
                                {
                                    sCurrent = sPrev + sCurrent;
                                    sPrev = null;
                                }
                                if (!startFound)
                                {
                                    if ((n = sCurrent.IndexOf("VALUES")) >= 0)
                                    {
                                        sCurrent = sCurrent.Substring(n + 6);
                                        startFound = true;
                                    }
                                    else
                                    {
                                        sPrev = sCurrent;
                                        continue;
                                    }
                                }
                                while (true)
                                {
                                    if (cnt == 0)
                                        n = sCurrent.IndexOf('(');
                                    else
                                        n = sCurrent.IndexOf("\'),(");
                                    if (n >= 0)
                                    {
                                        if (cnt > 0)
                                            n += 3;
                                        k = sCurrent.IndexOf("\')", n);
                                    }
                                    else
                                        k = -1;
                                    if (k < 0 || n < 0)
                                    {
                                        sPrev = sCurrent;
                                        break;
                                    }

                                    cmd.CommandText = "INSERT INTO d_zipcode_info VALUES " + sCurrent.Substring(n, k - n + 2);
                                    cmd.ExecuteNonQuery();
                                    cnt++;
                                    sCurrent = sCurrent.Substring(k);
                                }
                            }
                            cmd.Transaction.Commit();
                        }
                        catch
                        {
                            cmd.Transaction.Rollback();
                            throw;
                        }
                    }
                }
                Console.WriteLine("Completed. {0} line(s)", cnt);
                Console.ReadKey(true);
            }
          * */
            /*
            List<String> sData = new List<string>();
            String[] sLine;
            int n = 0, cnt;
            using (var sr = new StreamReader("D:\\INPUT.txt"))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    sLine = sr.ReadLine().Split(new Char[] { ',' }, 5, StringSplitOptions.RemoveEmptyEntries);
                    if (sLine.Length < 5)
                        continue;
                    sData.Add(sLine[4]);
                }
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var dbl = sData.Where(t =>
            {
                double d;
                return Double.TryParse(t, out d);
            })
            .Select(t => Double.Parse(t)).ToList();
            Double mx = dbl.Max(), mn = dbl.Min();
            cnt = dbl.Count;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            using (var sw = new StreamWriter(File.Open("D:\\OUTPUT.txt", FileMode.Create, FileAccess.Write)))
            {
                while (true)
                {
                    var data = dbl.Skip(n++).Take(LEN).ToArray();
                    if (data.Length < LEN)
                        break;
                    
                    sw.Write(data.First());
                    foreach (var s in data.Skip(1))
                    {
                        sw.Write('\t');
                        sw.Write(s);
                    }
                    sw.WriteLine();
                }
            }*/
        }
    }
}
