﻿namespace Common.Database.Procedure;

public class spSelectSerialCouponCode(string serialCode) : BaseStoredProcedure
{
    public SerialCouponCodeDao SerialCouponCode { get; set; }

    protected override bool Query()
    {
        try
        {
            var collection = GetCollection<SerialCouponCodeDao>();
            SerialCouponCode = collection.Find(SerialCouponCodeDao.FilterDefinition(serialCode)).FirstOrDefault();

            return true;
        }
        catch (Exception ex) 
        {
            LogSystem.Log.Error(ex);

            return false;
        }
    }

    protected override async Task<bool> QueryAsync()
    {
        try
        {
            var collection = GetCollection<SerialCouponCodeDao>();
            SerialCouponCode = await (await collection.FindAsync(SerialCouponCodeDao.FilterDefinition(serialCode))).FirstOrDefaultAsync();

            return true;
        }
        catch (Exception ex)
        {
            LogSystem.Log.Error(ex);

            return false;
        }
    }
}