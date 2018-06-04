using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace AddBadWordBot
{
    class ChatBot
    {
        static string conStr = @"Data Source = .\SQLEXPRESS; Initial Catalog = Dictionaries; Integrated Security = true;";
        static SqlConnection sqlConnection  = new SqlConnection(conStr);

        static string question;

        public event Action<string> GetStr; // событие

        public ChatBot()
        {
            try
            {
                sqlConnection.Open();
            }
            catch (Exception) { }

            GetStr += ChatBot_GetStr;
            
        }

        public void GenAnswer (string q)
        {
            
            question = q;
            string answer = Answer(question);
            if (answer == string.Empty)
            {
                Teach();
                GetStr("Запомнил! Давай еще!");
                GetStr += ChatBot_GetStr;
            }
            else
            {
                GetStr("Такое я уже знаю! Давай что-нибудь пооригинальнее "); //возвращаем ответ
            }

        }

        void Teach()
        {
            SqlCommand command = new SqlCommand("INSERT INTO BadWords (BadWord) VALUES ('" + question + "')", sqlConnection);
            command.ExecuteNonQuery();
        }

       
        public string Answer(string q) 

        {
            string answer = string.Empty,

            symbols = "`~!@#$%^&*(){}[]_-+=|\\|/?.,<>'\"№0 ";
            question=Exclude(q.ToLower(),symbols.ToCharArray()); //унификация сиволов ответа

            SqlCommand command = new SqlCommand("SELECT BadWord FROM BadWords WHERE BadWord = '" + question + "';",sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            
            while (reader.Read())
            {
            //}
            //if (reader[0].ToString().Length != 0)
            //{
                if (reader[0].ToString() == question)
                {
                    Console.WriteLine(reader[0].ToString());
                    answer = reader[0].ToString();
                }
            }
            reader.Close();
           
            return answer; // при проверке стоит поставить точку останова
        }

        static string Exclude(string q, char[] symbols)
        {
            foreach (char symbol in symbols)
            {
                q = q.Replace(symbol.ToString(), string.Empty);
            }
            
            return q; // при проверке стоит поставить точку останова
        }

        void ChatBot_GetStr(string obj)
        {
            // заглушка
        }
    }
}
