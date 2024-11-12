using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityTools.DbContextBackedService.Test.DbContextBackedService.Helpers;

public class EntityBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    /// <summary>
    ///  used to permanently doom this instance as a dummy, which means it's just used for value comparisons
    /// </summary>
    [NotMapped]
    public bool IsDummy { get; private set; } = false;

    public void TurnIntoDummy()
    {
        Id = int.MinValue;
        IsDummy = true;
    }

    /// <summary>
    /// TODO: ...
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="constr"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T GenerateDummy<T>(params object[] constr) where T : EntityBase
    {
        // activator generate instance of any ...
        var x = (T)(Activator.CreateInstance(typeof(T), constr) ?? throw new ArgumentException());
        x.TurnIntoDummy();
        return x;
    }
}