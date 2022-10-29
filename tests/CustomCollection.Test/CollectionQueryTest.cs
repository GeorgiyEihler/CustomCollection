using CustomCollecion.CollectionQuery;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit.Abstractions;

namespace CustomCollection.Test;

public class CollectionQueryTest
{
    private static Random Rnd = new Random();
    public record Person 
    {
        public int Age { get; init; }
        public string? Name { get; init; }
        public string? Address { get; init; }

    }

    public record NewPerson
    {
        public string? Name { get; init; }

    }

    internal class PersonEqual : EqualityComparer<Person>
    {
        public override bool Equals(Person? x, Person? y)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode([DisallowNull] Person obj)
        {
            throw new NotImplementedException();
        }
    }

    internal class PersonNameComparor : IComparer<Person>
    {
        public int Compare(Person? x, Person? y)
        {
            if(x is null || y is null || x.Name is null || y.Name is null)
            {
                throw new NullReferenceException();
            }
            if(x.Name![0] == y.Name![0])
            {
                return 0;
            }
            if(x.Name![0] > y.Name![0])
            {
                return 1;
            }

            return -1;
        }

    }

    internal class PersonAgeDescComparor : IComparer<Person>
    {
        public int Compare(Person? x, Person? y)
        {
            if (x is null || y is null)
            {
                throw new NullReferenceException();
            }
            if (x.Age == y.Age)
            {
                return 0;
            }
            if (x.Name![0] < y.Name![0])
            {
                return 1;
            }

            return -1;
        }

    }

    public class CollectionsToTests<T> : IXunitSerializable
    {
        private T[] _results;
        private IList<T> _list;

        public CollectionsToTests()
        {

        }

        public CollectionsToTests(T[] results)
        {
            _results = results;
            _list = results.ToList();
        }

        public IList<T> Build()
        {
            return _list;
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            info.GetValue<T>("_source");

            _results = info.GetValue<T[]>("_results");

            _list = _results.ToList();
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("_results", _results, typeof(T[]));
        }
    }

    public static Person GetPerson()
    => new()
    {
        Age = Rnd.Next(1, int.MaxValue),
        Name = Convert.ToBase64String(Encoding.UTF8.GetBytes(Rnd.Next(1, int.MaxValue).ToString())),
        Address = Convert.ToBase64String(Encoding.UTF8.GetBytes(Rnd.Next(1, int.MaxValue).ToString()))
    };

    public static IList<Person> GetPersons(int count)
    {
        var list = new List<Person>();
        for (int i = 0; i < count; i++)
        {
            list.Add(GetPerson());
        }

        return list;    
    }

    /// <summary>
    /// Получить рандомный уникальный массив со значениями
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static object[] GetItems(int count)
    {
        var items = new object[count];

        for (var i = 0; i < count; i++)
        {
            items[i] = (Convert.ToBase64String(Encoding.UTF8.GetBytes(i.ToString())));
        }

        return items;
    }

    /// <summary>
    /// Получить рандомный уникальный лист со значениями
    /// </summary>
    /// <param name="count"></param>
    /// <param name="listsToTest"></param>
    /// <returns></returns>
    public static IEnumerable<object[]> GetRandomUniqueHumanList(int count, int listsToTest)
        {
            for (int iList = 0; iList < listsToTest; iList++)
            {
                var collection = new Person[count];

                for (var i = 0; i < count; i++)
                {
                
                    collection[i] = GetPerson();
                }

                yield return new object[] {
                    new CollectionsToTests<Person>(collection)
                };

            };
        }

    /// <summary>
    /// Получить рандомный уникальный лист со значениями людей
    /// </summary>
    /// <param name="count"></param>
    /// <param name="listsToTest"></param>
    /// <returns></returns>
    public static IEnumerable<object[]> GetRandomUniqueList(int count, int listsToTest)
    {
        for (int iList = 0; iList < listsToTest; iList++)
        {
            var prevValue = 0D;

            var list = new string[count];

            for (var i = 0; i < count; i++)
            {
                list[i] = Convert.ToBase64String(Encoding.UTF8.GetBytes(prevValue++.ToString()));
            }

            yield return new object[] {
                new CollectionsToTests<string>(list)
            };

        };
    }

    [Fact]
    public void Filter_Empty()
    {
        var list = new List<object>();

        var result = list.Filter(null);

        Assert.Throws<NullReferenceException>(()=> result.ToCustomList());
    }

    [Fact]
    public void Filter_Numbers()
    {
        var list = new List<int>()
        {
            1, 2, 3, 4, 5, 6, 7,
        };

        var result = list.Filter(x=> x == 5);

        var count = result.Count();

        var item = result.First();

        Assert.Equal(1, count);

        Assert.Equal(5, item);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    public void Filter_Objects(int count)
    {
        var list = GetPersons(count);

        var item = list[list.Count >> 1];

        var filteredItems = list.Filter(x=> x.Age == item.Age);

        var persons = 0;

        foreach (var element in filteredItems)
        {
            persons++;
            Assert.Equal(item.Age, element.Age);
        }
        Assert.True(persons > 0);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    public void Filter_NotFound(int count)
    {
        var list = GetPersons(count);

        var filteredItems = list.Filter(x => x.Age == -100);

        var persons = 0;    

        foreach (var element in filteredItems)
        {
            persons++;
        }
        Assert.True(persons == 0);
    }

    [Fact]
    public void Get_Empty()
    {
        var list = new List<object>();

        IEnumerable<int> result = list.Get<object, int>(null);

        Assert.Throws<NullReferenceException>(() => result.ToCustomList());
    }

    [Fact]
    public void Get_Anonumus()
    {
        var list = GetPersons(128);

        var result = list.Get(x=> new { name = x.Name, age = x.Age });

        var index = 0;

        foreach (var item in result)
        {
            Assert.Equal(list[index].Name, item.name);
            Assert.Equal(list[index].Age, item.age);
            index++;
        }

        Assert.Equal(list.Count, index);
    }

    [Fact]
    public void Get_NewType()
    {
        var list = GetPersons(128);

        var result = list.Get(x => new NewPerson() { Name = x.Name});

        var index = 0;

        foreach (var item in result)
        {
            Assert.Equal(list[index].Name, item.Name);
            Assert.IsType<NewPerson>(item);
            
            index++;
        }

        Assert.Equal(list.Count, index);
    }

    [Fact]
    public void Sort_Empty()
    {
        var list = new List<object>();

        var result = list.SortQuery<object, int>(null);

        Assert.Throws<NullReferenceException>(() => result.ToCustomList());
    }

    [Fact]
    public void Sort_Numbers()
    {
        var expected = Enumerable.Range(0, 10).ToList();

        var list = new List<object>() { 4, 9, 0, 8, 1, 3, 5, 6, 7, 2};

        var sordetList = list.SortQuery(x => x).ToList();

        for (int i = 0; i < sordetList.Count; i++)
        {
            Assert.Equal(expected[i], sordetList[i]);
        }

        Assert.Equal(expected.Count, sordetList.Count);
    }

    [Fact]
    public void Sort_Strings()
    {
        var expected = Enumerable.Range(0, 10).Get(x=> x.ToString()).ToList();

        var list = new List<string>() 
        { 
            expected[4]!,
            expected[9]!,
            expected[0]!,
            expected[8]!,
            expected[1]!,
            expected[3]!,
            expected[5]!,
            expected[6]!,
            expected[7]!,
            expected[2]!
        };

        var sordetList = list.SortQuery(x => x).ToList();

        for (int i = 0; i < sordetList.Count; i++)
        {
            Assert.Equal(expected[i], sordetList[i]);
        }

        Assert.Equal(expected.Count, sordetList.Count);
    }

    [Fact]
    public void Sort_Object()
    {
        var expected = Enumerable.Range(0, 10).Get(x => new Person { Address = "", Age = x, Name = "" }).ToList();

        var list = new List<Person>()
        {
            expected[4]!,
            expected[9]!,
            expected[0]!,
            expected[8]!,
            expected[1]!,
            expected[3]!,
            expected[5]!,
            expected[6]!,
            expected[7]!,
            expected[2]!
        };

        var sordetList = list.SortQuery(x => x.Age).ToList();

        for (int i = 0; i < sordetList.Count; i++)
        {
            Assert.Equal(expected[i].Age, sordetList[i].Age);
        }

        Assert.Equal(expected.Count, sordetList.Count);
    }

    [Fact]
    public void Sort_ObjectCustomComparer()
    {
        var expected = Enumerable.Range(0, 10).Get(x => new Person { Address = "", Age = x, Name = x.ToString() }).ToList();

        var list = new List<Person>()
        {
            expected[4]!,
            expected[9]!,
            expected[0]!,
            expected[8]!,
            expected[1]!,
            expected[3]!,
            expected[5]!,
            expected[6]!,
            expected[7]!,
            expected[2]!
        };

        var sordetList = list.SortQuery(x => x, new PersonNameComparor()).ToList();

        for (int i = 0; i < sordetList.Count; i++)
        {
            Assert.Equal(expected[i].Age, sordetList[i].Age);
        }

        Assert.Equal(expected.Count, sordetList.Count);
    }

}
