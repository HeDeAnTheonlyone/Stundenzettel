using Godot.Collections;

public struct Customer
{
    public string Name { get; set; }
    public string Town { get; set; }
    public string Street { get; set; }



    public Customer(string name = "", string town = "", string street = "")
    {
        Name = name;
        Town = town;
        Street = street; 
    }



    public Customer(Dictionary dict)
    {
        Name = (string)dict["name"];
        Town = (string)dict["town"];
        Street = (string)dict["street"];
    }



    public Dictionary ToDict() => new Dictionary
    {
        { "name", Name },
        { "town", Town },
        { "street", Street }
    };
}