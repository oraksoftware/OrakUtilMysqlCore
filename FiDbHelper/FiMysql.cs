using MySqlConnector;
using OrakUtilDotNetCore.FiConfig;
using OrakUtilDotNetCore.FiContainer;
using System.Data;

namespace OrakUtilMysqlCore.FiDbHelper;

public class FiMysql
{
  private string? connString { get; set; }

  //public MySqlConnection conn { get; private set; }
  //public MySqlCommand comm { get; private set; }

  public FiMysql(string? connString)
  {
    this.connString = connString;
    //conn = new MySqlConnection(this.connString);
    //comm = conn.CreateCommand();
  }

  public bool TestConnection()
  {
    try
    {
      using (MySqlConnection testConn = new MySqlConnection(connString))
      {
        testConn.Open();
        return true;
      }
    }
    catch
    {
      return false;
    }
  }

  private static MySqlParameter[] ProcessParameters(FiKeybean fkbParams)
  {
    MySqlParameter[] pars = fkbParams.Select(pair => new MySqlParameter()
    {
      ParameterName = pair.Key, Value = pair.Value
    }).ToArray();

    return pars;
  }



  public virtual Fdr ExecQuery(string query, FiKeybean? parameters)
  {
    Fdr fdrResult = new Fdr();

    using MySqlConnection conn = new MySqlConnection(connString);
    using MySqlCommand comm = conn.CreateCommand();

    comm.Parameters.Clear();
    comm.CommandText = query;
    comm.CommandType = CommandType.Text;

    if (parameters is { Count: > 0 })
    {
      comm.Parameters.AddRange(ProcessParameters(parameters));
    }

    int result = 0;

    conn.Open();
    try
    {
      result = comm.ExecuteNonQuery();
      // Rows affected is -1 for statements that do not affect rows
      if (result == -1) result = 1;
      fdrResult.boResult = true;
      fdrResult.lnRowsAffected = result;
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      //FiAppConfig.fiLog?.Error();
      result = -2;
      fdrResult.boResult = false;
      fdrResult.refException = e;
    }
    finally
    {
      conn.Close();
    }

    return fdrResult;
  }

  public Fdr SelectDtb(string query, FiKeybean? parameters)
  {
    Fdr fdrResult = new Fdr();

    using MySqlConnection conn = new MySqlConnection(this.connString);
    using MySqlCommand comm = conn.CreateCommand();

    comm.Parameters.Clear();
    comm.CommandText = query;
    comm.CommandType = CommandType.Text;

    if (parameters is { Count: > 0 })
    {
      comm.Parameters.AddRange(ProcessParameters(parameters));
    }

    // Adaptor : otomatik bağlantı açar. Verileri çeker(sorguyu çalıştırır)
    // ve bir datatable 'a doldurur ve bağlantıyı otomatik kapatır.
    MySqlDataAdapter da = new MySqlDataAdapter(comm);

    try
    {
      DataTable dt = new DataTable();
      da.Fill(dt);
      fdrResult.boResult = true;
      fdrResult.refDtbVal = dt;
      return fdrResult;
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      fdrResult.boResult = false;
      fdrResult.refException = e;
      // Optionally, return null or handle differently
      return fdrResult;
    }
  }

  public virtual DataTable ExecProcDtb(string procName, FiKeybean parameters) //params ParamItem[] parameters
  {
    using MySqlConnection conn = new MySqlConnection(this.connString);
    using MySqlCommand comm = conn.CreateCommand();

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