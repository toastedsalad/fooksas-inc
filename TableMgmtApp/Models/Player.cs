namespace TableMgmtApp;

public class Player {
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public int Discount { get; private set; }

    public Player(string name, string surname, string email, int discount) {
        Id = Guid.NewGuid();
        Name = name;
        Surname = surname;
        Email = email;
        Discount = discount;
    }
}
