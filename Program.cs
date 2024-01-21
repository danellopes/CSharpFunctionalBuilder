using System.Runtime.Intrinsics.Arm;

namespace FunctionalBuilder;

public class Person
{
    public string Name, Position, Hobby;

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Position)}: {Position}, {nameof(Hobby)}: {Hobby}";
    }
}

public abstract class FunctionalBuilder<TSubject, TSelf>
where TSelf : FunctionalBuilder<TSubject, TSelf>
where TSubject : new()
{
    private readonly List<Func<Person, Person>> Actions = new List<Func<Person, Person>>();

    public TSelf Do(Action<Person> action) => AddAction(action);

    public Person Build() => Actions.Aggregate(new Person(), (p, f) => f(p));

    private TSelf AddAction(Action<Person> action)
    {
        Actions.Add(p =>
        {
            action(p);
            return p;
        });

        return (TSelf)this;
    }
}

public sealed class PersonBuilder : FunctionalBuilder<Person, PersonBuilder>
{
    public PersonBuilder Called(string name) => Do(p => p.Name = name);
}

public static class PersonBuilderExtensions
{
    public static PersonBuilder WorksAs(this PersonBuilder builder, string position) => builder.Do(p => p.Position = position);
    public static PersonBuilder HasFunWith(this PersonBuilder builder, string hobby) => builder.Do(p => p.Hobby = hobby);
}

class Program
{
    static void Main(string[] args)
    {
        var person = new PersonBuilder()
        .Called("Sarah")
        .WorksAs("Developer")
        .HasFunWith("Video games")
        .Build();

        System.Console.WriteLine(person);
    }
}
