using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Collections;
using System.Composition;
using System.Diagnostics;

namespace Blockbuster_Rental_Software.Models
{
    public class DB
    {
        private MySqlConnection connection = new MySqlConnection("Server=localhost; port=1433;User ID=root;Password=Specialpotato1*;Database=georgedatabase");

        public List<object> QueryRow(string query, List<string> param, List<string> value)
        {
            try
            {
                var response = new List<object>();
                connection.Open();
                var command = new MySqlCommand(query, connection);
                if(!param.IsNullOrEmpty())
                {
                    for(int i = 0; i < param.Count(); i++)
                    {
                        command.Parameters.AddWithValue(param[i], value[i]);
                    }
                }                

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        response.Add(reader[j]);
                    }
                }

                
         


                reader.Close();
                connection.Close();
                return response;
            }
            catch (Exception ex)
            {
                var response = new List<object>();
                response.Add(ex.Message);
                return response;
            }
            
        }

        public List<object> QueryRow(string query)
        {
            try
            {
                var response = new List<object>();
                connection.Open();
                var command = new MySqlCommand(query, connection);

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        response.Add(reader[j]);
                    }
                }

                reader.Close();
                connection.Close();
                return response;
            }
            catch (Exception ex)
            {
                var response = new List<object>();
                response.Add(ex.Message);
                return response;
            }

        }

        public string NoDataQuery(string query, List<string> param, List<string> value)
        {
            try
            {
                connection.Open();
                var command = new MySqlCommand(query, connection);
                if (!param.IsNullOrEmpty())
                {
                    for (int i = 0; i < param.Count(); i++)
                    {
                        command.Parameters.AddWithValue(param[i], value[i]);
                    }
                }
                command.ExecuteScalar();

                connection.Close();
                return "";
            } catch (Exception ex)
            {
                return ex.Message;
            }
            
            
        }

        public string NoDataQuery(string query)
        {
            try
            {
                connection.Open();
                var command = new MySqlCommand(query, connection);

                command.ExecuteScalar();

                connection.Close();
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }









    }
}
