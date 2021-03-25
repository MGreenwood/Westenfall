using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public delegate void LoginAttempt(string username); // client should maintain an implementation that is called upon successful login

    public static DatabaseManager instance;

    #region Database variables
    private static string _sqlDBLocation = "";

    /// <summary>
    /// Table name and DB actual file location
    /// </summary>
    private const string SQL_DB_NAME = "Database/GameDatabase";

    // table name
    private const string LOGIN_TABLE_NAME = "Login";

    //
    // Columns for Login Table
    //
    private const string COL_USERNAME = "username";  
    private const string COL_PASSWORD = "password";
    private const string COL_SALT = "salt";

    // 
    // Columns for Player Table
    //



    /// <summary>
    /// DB objects
    /// </summary>
    private IDbConnection _connection = null;
    private IDbCommand _command = null;
    private IDataReader _reader = null;
    private string _sqlString;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            string filepath = Application.dataPath + "/" + SQL_DB_NAME;

            _sqlDBLocation = "URI=file:" + filepath + ".db";

            if (!File.Exists(filepath + ".db"))
            {
                Debug.Log("DB not found, must be standalone.");

                /*
                filepath = Application.dataPath + "/" + SQL_DB_NAME;
                _sqlDBLocation = "URI=file:" + filepath + ".db";
                */
            }

        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SQLiteInit();
    }

    public bool CreateAccount(string username, byte[] hashedPassword, byte[] salt)
    {
        // select count where username = username
        _command.CommandText = $"SELECT COUNT(*) FROM {LOGIN_TABLE_NAME} WHERE username = @username";
        _command.Parameters.Add(new SqliteParameter("@username", username));
        _connection.Open();

        try
        {
            int count = Convert.ToInt32(_command.ExecuteScalar());
            if (count != 0)
            {
                Debug.Log("User already exists in the database");
                _connection.Close();
                return false;
            }

            _command.CommandText = $"INSERT INTO {LOGIN_TABLE_NAME} ({COL_USERNAME}, {COL_PASSWORD}, {COL_SALT}) VALUES (@username, @password, @salt)";
            _command.Parameters.Add(new SqliteParameter("@username", username));
            _command.Parameters.Add(new SqliteParameter("@password", hashedPassword));
            _command.Parameters.Add(new SqliteParameter("@salt", salt));

            _command.ExecuteNonQuery();
        }
        catch(Exception e)
        {
            Debug.Log("Account creation failed: " + e.Message);
            _connection.Close();
            return false;
        }


        Debug.Log("Account created successfully");
        _connection.Close();
        return true;
    }

    public byte[] GetUserSalt(string username)
    {
        byte[] salt = null;

        string commandText = $"SELECT {COL_SALT} FROM {LOGIN_TABLE_NAME} WHERE {COL_USERNAME} = @username";
        _command.CommandText = commandText;

        _command.Parameters.Add(new SqliteParameter("@username", username));

        _connection.Open();
        try
        {
            salt = (byte[])_command.ExecuteScalar();
        }
        catch (Exception e)
        {
            Debug.Log("Error logging in: " + e.Message);
        }

        _connection.Close();

        return salt;
    }
    
    public void Login(string username, byte[] hashedPassword, LoginAttempt loginMethod)
    {
        byte[] password = new byte[32];

        string commandText = $"SELECT {COL_PASSWORD} FROM {LOGIN_TABLE_NAME} WHERE {COL_USERNAME} = @username";
        _command.CommandText = commandText;

        _command.Parameters.Add(new SqliteParameter("@username", username));

        _connection.Open();
        try
        {
            using( _reader = _command.ExecuteReader())
            {
                _reader.Read();
                password = GetBytes();
            }

            if(password.SequenceEqual(hashedPassword))
            {
                loginMethod?.Invoke(username);
            }
        }
        catch(Exception e)
        {
            Debug.Log("Error logging in: " + e.Message);
        }

        _connection.Close();
    }

    public void SaveAllPlayerData(string username, string characterName, PlayerClass.ClassType classType, int level, Player.AppliedAttributes appliedAttributes, Inventory inventory, Equipment equipment, Inventory sharedInventory)
    {

    }

    public void RetrievePlayerData(ref PlayerClass.ClassType classType, ref int level, ref Player.AppliedAttributes appliedAttributes, 
                                   ref Inventory inventory, ref Equipment equipment, ref Inventory sharedInventory)
    {

    }

    byte[] GetBytes()
    {
        const int CHUNK_SIZE = 2 * 1024;
        byte[] buffer = new byte[CHUNK_SIZE];
        long bytesRead;
        long fieldOffset = 0;
        using (MemoryStream stream = new MemoryStream())
        {
            while ((bytesRead = _reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, (int)bytesRead);
                fieldOffset += bytesRead;
            }
            return stream.ToArray();
        }
    }

    byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Basic initialization of SQLite
    /// </summary>
    private void SQLiteInit()
    {
        _connection = new SqliteConnection(_sqlDBLocation);
        _command = _connection.CreateCommand();
        _connection.Open();

        // WAL = write ahead logging, very huge speed increase
        _command.CommandText = "PRAGMA journal_mode = WAL;";
        _command.ExecuteNonQuery();

        // journal mode = look it up on google, I don't remember
        _command.CommandText = "PRAGMA journal_mode";
        _reader = _command.ExecuteReader();
        if (GameManager.instance.Debug && _reader.Read())
            Debug.Log("SQLiter - WAL value is: " + _reader.GetString(0));
        _reader.Close();

        // more speed increases
        _command.CommandText = "PRAGMA synchronous = OFF";
        _command.ExecuteNonQuery();

        // and some more
        _command.CommandText = "PRAGMA synchronous";
        _reader = _command.ExecuteReader();
        if (GameManager.instance.Debug && _reader.Read())
            Debug.Log("SQLiter - synchronous value is: " + _reader.GetInt32(0));
        _reader.Close();
        
        _connection.Close();
    }

}
