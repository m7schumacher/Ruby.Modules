//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data.OleDb;
//using System.Data.SqlClient;
//using System.Data;
//using System.Data.Sql;
//using Ruby//using Ruby.Immune;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Data.Sql;
//using MySql.Data;

//namespace Ruby//{
//    internal class Connector
//    {
//        OleDbConnection connection;
//        OleDbCommand command;

//        private void ConnectTo()
//        {
//            MySql.Data.MySqlClient.MySqlConnection conn;
//            string myConnectionString;

//            myConnectionString = "Server=134.129.22.147;Port=3306;Database=Ruby_DB;Uid=root@localhost;PASSWORD=Bison28!;";

//            try
//            {
//                conn = new MySql.Data.MySqlClient.MySqlConnection();
//                conn.ConnectionString = myConnectionString;
//                conn.Open();
//            }
//            catch (MySql.Data.MySqlClient.MySqlException ex)
//            {
//                MessageBox.Show(ex.Message);
//            }

//            //connection = new OleDbConnection(@"Provider=SQLOLEDB.1;Persist Security Info=False;Data Source=C:\Users\mschuee\Documents\Visual Studio 2013\Projects\Ruby\Ruby\Ruby_DB.mdb");
//            //command = connection.CreateCommand();
//        }

//        public Connector()
//        {
//            ConnectTo();
//        }

//        public void Insert(Killer_TCell phage)
//        {
//            try
//            {
//                command.CommandText = "INSERT INTO Macrophage (ID, KeyWords, Target)" +
//                    "VALUES('" + phage.getID() + "', '" + phage.getKeyWords() + "', '" + phage.Target +"')";

//                command.CommandType = CommandType.Text;

//                connection.Open();

//                command.ExecuteNonQuery();

//            }
//            catch (Exception trouble)
//            {
//                MessageBox.Show(trouble.Message);
//            }
//            finally
//            {
//                if(connection != null)
//                {
//                    connection.Close();
//                }
//            }
//        }
//    }
//}
