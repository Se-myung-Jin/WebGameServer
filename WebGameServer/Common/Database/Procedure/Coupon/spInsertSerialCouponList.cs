namespace Common.Database.Procedure;

public class spInsertSerialCouponList(List<SerialCouponCodeDao> SerialCouponCodeList) : BaseStoredProcedure
{
    public int InsertCount { get; set; } = SerialCouponCodeList.Count;

    protected override bool Query()
    {
        try
        {
            var SerialCouponCodeCollection = GetCollection<SerialCouponCodeDao>();
            SerialCouponCodeCollection.InsertMany(SerialCouponCodeList);

            return true;
        }
        catch (MongoBulkWriteException<SerialCouponCodeDao> ex)
        {
            InsertCount = (int)ex.Result.InsertedCount;

            return false;
        }
    }

    protected override async Task<bool> QueryAsync()
    {
        try
        {

            var SerialCouponCodeCollection = GetCollection<SerialCouponCodeDao>();
            await SerialCouponCodeCollection.InsertManyAsync(SerialCouponCodeList);

            return true;
        }
        catch (MongoBulkWriteException<SerialCouponCodeDao> ex)
        {
            InsertCount = (int)ex.Result.InsertedCount;

            return false;
        }
    }
}