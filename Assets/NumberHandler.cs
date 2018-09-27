
using System.Collections.Generic;
using System.Linq;

public class BurglarAlarmHelper
{
    private readonly IList<int> tokenizedModuleNumber = new List<int>();
    private readonly KMBombInfo kmBombInfo;
    private readonly bool? isEvenNoSolvedModules;
    private readonly bool? isSolvedGreaterBateriesTimesPortPlates;

    public BurglarAlarmHelper(IList<int> tokenizedModuleNumber, KMBombInfo kmBombInfo) :
        this(tokenizedModuleNumber, kmBombInfo, null, null)
    {
    }

    /// <summary>
    /// Initialize helper class for the burglar alarm module.
    /// </summary>
    /// <param name="tokenizedModuleNumber">The number, as a list of integers.</param>
    /// <param name="kmBombInfo">KMBomb information.</param>
    /// <param name="isEvenNoSolvedModules">Nullable bool for overriding the default value for number of solved modules.</param>
    /// <param name="isSolvedGreaterBateriesTimesPortPlates">Nullable bool for overiding the default value of 
    /// number of solved greater than number of batteries times port plates.</param>
    /// <remarks>The two nullable bools should only be used for log debug purposes.</remarks>
    public BurglarAlarmHelper(IList<int> tokenizedModuleNumber, KMBombInfo kmBombInfo, bool? isEvenNoSolvedModules, bool? isSolvedGreaterBateriesTimesPortPlates)
    {
        this.tokenizedModuleNumber = tokenizedModuleNumber;
        this.kmBombInfo = kmBombInfo;
        this.isEvenNoSolvedModules = isEvenNoSolvedModules;
        this.isSolvedGreaterBateriesTimesPortPlates = isSolvedGreaterBateriesTimesPortPlates;
    }

    public string ToStringNumber
    {
        get
        {
            return string.Join(string.Empty, this.tokenizedModuleNumber.Select(x => x.ToString()).ToArray());
        }
    }

    public int GetModuleNumber(int i)
    {
        return this.tokenizedModuleNumber[i];
    }

    public bool IsModuleNumberEven(int i)
    {
        return this.IsEvenNumber(this.tokenizedModuleNumber[i]);
    }

    public int GetPortPlatesCount
    {
        get
        {
            return this.kmBombInfo.GetPortPlateCount();
        }
    }

    public bool IsEvenNumberOfPorts
    {
        get
        {
            return this.IsEvenNumber(this.GetPortCount);
        }
    }

    public bool IsTotalModuleNumberOdd
    {
        get 
        {
            return !this.IsEvenNumber(this.tokenizedModuleNumber.Sum());
        }
    }

    public bool NumberOfsolvedIsGraterThanNumberOfBatteriesXPortplates
    {
        get
        {
            return this.isSolvedGreaterBateriesTimesPortPlates ?? this.kmBombInfo.GetSolvedModuleNames().Count() > this.GetBatteryCount * this.GetPortPlatesCount;
        }
    }

    public bool IsEvenNumberOfSolved
    {
        get
        {
            return this.isEvenNoSolvedModules ?? this.IsEvenNumber(this.kmBombInfo.GetSolvedModuleNames().Count);
        }
    }

    public int GetBatteryCount
    {
        get
        {
            return this.kmBombInfo.GetBatteryCount();
        }
    }

    public int GetPortCount
    {
        get
        {
            return this.kmBombInfo.GetPortCount();
        }
    }

    public bool IsPortsMoreThanIndicators
    {
        get
        {
            return this.GetPortCount > this.kmBombInfo.GetIndicators().Count();
        }
    }


    public bool IsEvenNumberOfBatteryHolders
    {
        get
        {
            return this.GetBatteryHolderCount % 2 == 0;
        }
    }

    public int GetBatteryHolderCount
    {
        get
        {
            return this.kmBombInfo.GetBatteryHolderCount();
        }

    }

    public bool HasParallelPort
    {
        get
        {
            return this.HasPortType(KMBombInfoExtensions.KnownPortType.Parallel);
        }
    }

    public bool HasSerialPort
    {
        get
        {
            return this.HasPortType(KMBombInfoExtensions.KnownPortType.Serial);
        }
    }

    public bool HasPs2Port
    {
        get
        {
            return this.HasPortType(KMBombInfoExtensions.KnownPortType.PS2);
        }
    }

    public bool HasRJ45Port
    {
        get
        {
            return this.HasPortType(KMBombInfoExtensions.KnownPortType.RJ45);
        }
    }
    
    public bool AreMorePortPlatesThanIndicators
    {
        get
        {
            return this.kmBombInfo.GetIndicators().Count() < this.kmBombInfo.GetPortPlateCount();
        }
    }  

    public int GetLitIndicators
    {
        get
        {
            return this.kmBombInfo.GetOnIndicators().Count();
        }
    }

    public bool AreNoLitIndicators
    {
        get
        {
            return this.GetLitIndicators == 0;
        }
    }

    public bool IsBobIndicatorOn
    {
        get
        {
            return this.kmBombInfo.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.BOB);
        }
    }

    public int GetNumberOfIndicators
    {
        get
        {
            return this.kmBombInfo.GetIndicators().Count();
        }
    }

    public int GetUnlitIndicators
    {
        get
        {
            return this.kmBombInfo.GetOffIndicators().Count();
        }
    }

    public bool SerialNumberContainsBURGL14R
    {
        get
        {
            return this.kmBombInfo.GetSerialNumber().Any(x => new[] { 'B', 'U', 'R', 'G', '1', '4', 'R' }.Contains(x));
        }
    }

    public bool SerialNumberContainsAL53M
    {
        get
        {
            return this.kmBombInfo.GetSerialNumber().Any(x => new[] { 'A', 'L', '5', '3', 'M' }.Contains(x));
        }
    }

    public bool AreNoUnlitIndicators
    {
        get
        {
            return this.GetUnlitIndicators == 0;
        }
    }

    public bool IsFRQIndicatorOn
    {
        get
        {
            return this.kmBombInfo.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.FRQ);
        }
    }

    public bool AreMoreLettersThanDigitsInSerialNumber
    {
        get
        {
            return this.kmBombInfo.GetSerialNumberLetters().Count() > this.kmBombInfo.GetSerialNumberNumbers().Count();
        }
    }

    public bool AreThereMoreDBatteriesThanAABatteries
    {
        get
        {
            return this.kmBombInfo.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.D) > this.kmBombInfo.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AA) + this.kmBombInfo.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AAx3) + this.kmBombInfo.GetBatteryCount(KMBombInfoExtensions.KnownBatteryType.AAx4);
        }
    }



    public bool BatteriesAreMoreThan4
    {
        get
        {
            return this.GetBatteryCount > 4;
        }
    }

    public int AddToPosition(int i, int value)
    {
        var result = this.tokenizedModuleNumber[i] + value;
        return (result >= 10) ? result - 10 : result;
    }

    private bool IsEvenNumber(int number)
    {
        return number % 2 == 0;
    }

    private bool HasPortType(KMBombInfoExtensions.KnownPortType type)
    {
        return this.kmBombInfo.GetPortCount(type) > 0;
    }
}

public interface INumberHandler
{
    int GetNumber();
}

public class NumberHandlerPos1 : INumberHandler
{
    private const int MyIndex = 0;

    private BurglarAlarmHelper metadata;

    public NumberHandlerPos1(BurglarAlarmHelper metadata)
    {
        this.metadata = metadata;
    }

    public int GetNumber()
    {
        int value;
        if (metadata.GetBatteryCount > metadata.GetPortCount)
        {
            value = metadata.IsEvenNumberOfBatteryHolders ? 9 : 1;
        }
        else
        {
            value = metadata.IsModuleNumberEven(7) ? 3 : 4;
        }

        return metadata.AddToPosition(MyIndex, value);
    }
}

public class NumberHandlerPos2 : INumberHandler
{
    private const int MyIndex = 1;

    private BurglarAlarmHelper metadata;
    public NumberHandlerPos2(BurglarAlarmHelper metadata)
    {
        this.metadata = metadata;
    }

    public int GetNumber()
    {
        int value;
        if (metadata.HasPs2Port)
        {
            value = metadata.AreMoreLettersThanDigitsInSerialNumber ? 0 : 6;
        }
        else
        {
            value = metadata.IsBobIndicatorOn ? 5 : 2;
        }

        return metadata.AddToPosition(MyIndex, value);
    }
}

public class NumberHandlerPos3 : INumberHandler
{
    private const int MyIndex = 2;

    private BurglarAlarmHelper metadata;
    public NumberHandlerPos3(BurglarAlarmHelper metadata)
    {
        this.metadata = metadata;
    }

    public int GetNumber()
    {
        int value;
        if (metadata.IsEvenNumberOfSolved)
        {
            value = metadata.IsModuleNumberEven(2) ? 8 : 4;
        }
        else
        {
            value = metadata.HasRJ45Port ? 9 : 3;
        }

        return metadata.AddToPosition(MyIndex, value);
    }
}

public class NumberHandlerPos4 : INumberHandler
{
    private const int MyIndex = 3;

    private BurglarAlarmHelper metadata;
    public NumberHandlerPos4(BurglarAlarmHelper metadata)
    {
        this.metadata = metadata;
    }

    public int GetNumber()
    {
        int value;
        if (metadata.IsTotalModuleNumberOdd)
        {
            value = metadata.AreMorePortPlatesThanIndicators ? 7 : 3;
        }
        else
        {
            value = metadata.AreThereMoreDBatteriesThanAABatteries ? 7 : 2;
        }

        return metadata.AddToPosition(MyIndex, value);
    }
}


public class NumberHandlerPos5 : INumberHandler
{
    private const int MyIndex = 4;

    private BurglarAlarmHelper metadata;
    public NumberHandlerPos5(BurglarAlarmHelper metadata)
    {
        this.metadata = metadata;
    }

    public int GetNumber()
    {
        int value;
        if (metadata.NumberOfsolvedIsGraterThanNumberOfBatteriesXPortplates)
        {
            value = metadata.IsEvenNumberOfPorts ? 9 : 3;
        }
        else
        {
            value = metadata.IsPortsMoreThanIndicators ? 7 : 8;
        }

        return metadata.AddToPosition(MyIndex, value);
    }
}

public class NumberHandlerPos6 : INumberHandler
{
    private const int MyIndex = 5;

    private BurglarAlarmHelper metadata;
    public NumberHandlerPos6(BurglarAlarmHelper metadata)
    {
        this.metadata = metadata;
    }

    public int GetNumber()
    {
        int value;
        if (metadata.HasParallelPort)
        {
            value = metadata.HasSerialPort ? 1 : 5;
        }
        else
        {
            value = metadata.IsFRQIndicatorOn ? 0 : 4;
        }

        return metadata.AddToPosition(MyIndex, value);
    }
}

public class NumberHandlerPos7 : INumberHandler
{
    private const int MyIndex = 6;

    private BurglarAlarmHelper metadata;
    public NumberHandlerPos7(BurglarAlarmHelper metadata)
    {
        this.metadata = metadata;
    }

    public int GetNumber()
    {
        int value;
        if (metadata.BatteriesAreMoreThan4)
        {
            value = metadata.AreNoUnlitIndicators ? 2 : 6;
        }
        else
        {
            value = metadata.AreNoLitIndicators ? 4 : 9;
        }

        return metadata.AddToPosition(MyIndex, value);
    }
}

public class NumberHandlerPos8 : INumberHandler
{
    private const int MyIndex = 7;

    private BurglarAlarmHelper metadata;
    public NumberHandlerPos8(BurglarAlarmHelper metadata)
    {
        this.metadata = metadata;
    }

    public int GetNumber()
    {
        int value;
        if (metadata.GetNumberOfIndicators == metadata.GetBatteryCount)
        {
            value = metadata.SerialNumberContainsBURGL14R ? 1 : 0;
        }
        else
        {
            value = metadata.SerialNumberContainsAL53M ? 0 : 8;
        }

        return metadata.AddToPosition(MyIndex, value);
    }
}