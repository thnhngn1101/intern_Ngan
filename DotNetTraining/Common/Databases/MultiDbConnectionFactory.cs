using BPMaster.Common.Databases.Interfaces;
using System.Data;
public class BPMasterDbConnectionWrapper : IBPMasterDbConnection
{
    private readonly IDbConnection _innerConnection;

    public BPMasterDbConnectionWrapper(IDbConnection innerConnection)
    {
        _innerConnection = innerConnection;
    }

    public IDbTransaction BeginTransaction()
    {
        return _innerConnection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return _innerConnection.BeginTransaction(il);
    }

    public void ChangeDatabase(string databaseName)
    {
        _innerConnection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
        _innerConnection.Close();
    }

    public IDbCommand CreateCommand()
    {
        return _innerConnection.CreateCommand();
    }

    public void Open()
    {
        _innerConnection.Open();
    }

    public string ConnectionString
    {
        get => _innerConnection.ConnectionString;
        set => _innerConnection.ConnectionString = value;
    }

    public int ConnectionTimeout => _innerConnection.ConnectionTimeout;

    public string Database => _innerConnection.Database;

    public ConnectionState State => _innerConnection.State;

    public void Dispose()
    {
        _innerConnection.Dispose();
    }
}

public class WarehouseDbConnectionWrapper : IWarehouseDbConnection
{
    private readonly IDbConnection _innerConnection;

    public WarehouseDbConnectionWrapper(IDbConnection innerConnection)
    {
        _innerConnection = innerConnection;
    }

    public IDbTransaction BeginTransaction()
    {
        return _innerConnection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return _innerConnection.BeginTransaction(il);
    }

    public void ChangeDatabase(string databaseName)
    {
        _innerConnection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
        _innerConnection.Close();
    }

    public IDbCommand CreateCommand()
    {
        return _innerConnection.CreateCommand();
    }

    public void Open()
    {
        _innerConnection.Open();
    }

    public string ConnectionString
    {
        get => _innerConnection.ConnectionString;
        set => _innerConnection.ConnectionString = value;
    }

    public int ConnectionTimeout => _innerConnection.ConnectionTimeout;

    public string Database => _innerConnection.Database;

    public ConnectionState State => _innerConnection.State;

    public void Dispose()
    {
        _innerConnection.Dispose();
    }
}