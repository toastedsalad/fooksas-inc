namespace TableMgmtApp;

public class Player {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public int Discount { get; set; }

    public Player(string name, string surname, string email, int discount) {
        Id = Guid.NewGuid();
        Name = name;
        Surname = surname;
        Email = email;
        Discount = discount;
    }
}
