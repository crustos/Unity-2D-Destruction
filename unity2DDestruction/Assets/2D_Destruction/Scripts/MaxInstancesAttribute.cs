/// <summary>
/// At most N instances of this class exist at once. Unity ignores it; the
/// crust C# subset reads it: a [MaxInstances(N)] class is allocated from an
/// arena of N slots (a reference is a plain pointer, nothing is freed one at
/// a time), and unity_pack sizes the class's tables to N.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MaxInstancesAttribute : System.Attribute
{
    public MaxInstancesAttribute(int n) {}
}
