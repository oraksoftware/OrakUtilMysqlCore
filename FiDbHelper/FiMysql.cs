using MySqlConnector;
using OrakUtilDotNetCore.FiContainer;
using System.Data;

namespace OrakUtilMysqlCore.FiDbHelper;

public class FiMysql
{
  public string connString { get; private set; }
  public MySqlConnection conn { get; private set; }

  /// <summary>
  /// Comm : Command
  /// </summary>
  public MySqlCommand comm { get; private set; }


  public FiMysql(string connString)
  {
    this.connString = connString;
    //conn = new MySqlConnection(this.connString);
    //comm = conn.CreateCommand();
  }

  private static MySqlParameter[] ProcessParameters(FiKeybean fkbParams)
  {
    MySqlParameter[] pars = fkbParams.Select(pair => new MySqlParameter()
    {
      ParameterName = pair.Key, Value = pair.Value
    }).ToArray();

    return pars;
  }



  public virtual int RunQuery(string query, FiKeybean parameters)
  {
    comm.Parameters.Clear();
    comm.CommandText = query;
    comm.CommandType = CommandType.Text;

    if (parameters != null && parameters.Count > 0)
    {
      comm.Parameters.AddRange(ProcessParameters(parameters));
    }

    int result = 0;

    conn.Open();
    try
    {
      result = comm.ExecuteNonQuery();
      if (result == -1) result = 1;
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      result = -2;
      //throw;
    }

    conn.Close();

    return result;
  }

  public virtual DataTable RunProc(string procName, FiKeybean parameters) //params ParamItem[] parameters
  {
    comm.Parameters.Clear();
    comm.CommandText = procName;
    comm.CommandType = CommandType.StoredProcedure;

    if (parameters != null && parameters.Count > 0)
    {
      comm.Parameters.AddRange(ProcessParameters(parameters));
    }

    DataTable dt = new DataTable();
    MySqlDataAdapter adapter = new MySqlDataAdapter(comm);
    adapter.Fill(dt);

    return dt;
  }


  public virtual DataTable GetTable(string query, FiKeybean parameters) //params ParamItem[] parameters
  {
    comm.Parameters.Clear();
    comm.CommandText = query;
    comm.CommandType = CommandType.Text;

    if (parameters != null && parameters.Count > 0)
    {
      comm.Parameters.AddRange(ProcessParameters(parameters));
    }

    MySqlDataAdapter da = new MySqlDataAdapter(comm);

    // Adaptor : otomatik bağlantı açar. Verileri çeker(sorguyu çalıştırır) ve bir datatable 'a doldurur ve bağlantıyı otomatik kapatır.

    DataTable dt = new DataTable();
    da.Fill(dt);

    return dt;
  }
}


// private SqlParameter[] ProcessParameters(params ParamItem[] parameters)
// {
// 	SqlParameter[] pars = parameters.Select(x => new SqlParameter()
// 	{
// 		ParameterName = x.ParamName,
// 		Value = x.ParamValue
// 	}).ToArray();
//
// 	return pars;
// }

// public virtual int RunQuery(string query, params ParamItem[] parameters)
// {
// 	comm.Parameters.Clear();
// 	comm.CommandText = query;
// 	comm.CommandType = CommandType.Text;
//
// 	if (parameters != null && parameters.Length > 0)
// 	{
// 		comm.Parameters.AddRange(ProcessParameters(parameters));
// 	}
//
// 	int result = 0;
//
// 	conn.Open();
// 	try
// 	{
// 		result = comm.ExecuteNonQuery();
// 		if (result == -1) result = 1;
// 	}
// 	catch (Exception e)
// 	{
// 		Console.WriteLine(e);
// 		result = -2;
// 		//throw;
// 	}
//
// 	conn.Close();
//
// 	return result;
// }
