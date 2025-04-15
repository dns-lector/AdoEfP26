using AdoEfP26.Data;
using AdoEfP26.Data.Entities;
using Microsoft.EntityFrameworkCore;

DataContext dataContext = new();
String choice;
do
{
    Console.WriteLine("1 - Sign Up");
    Console.WriteLine("2 - Sign In");
    Console.WriteLine("0 - Exit");
    choice = Console.ReadLine()!;
    switch (choice)
    {
        case "1": SignUp(); break;
        case "2": SignIn(); break;
    }
} while (choice != "0");

void query1()
{
    var query = dataContext.Users.Where(u => u.Birthdate <= DateTime.Now);

    Console.WriteLine(query.Count());
}

void query2()
{
    var query = dataContext   // select * from Users join UserAccesses ON Users.Id = UserAccesses.UserId
        .Users                                        // Users
        .Join(                                        // join
            dataContext.UserAccesses,                 // UserAccesses
            u => u.Id,                                // ON Users.Id
            ua => ua.UserId,                          // = UserAccesses.UserId
            (u, ua) => new {User = u, Access = ua }   // select *
        );                                            // 
    foreach(var combo in query)
    {
        Console.WriteLine($"{combo.User.Name} {combo.Access.RoleId}");
    }
    Console.WriteLine("------------------");
    // Заміну поєднанням надають "навігаційні властивості"
    foreach(var ua in dataContext.UserAccesses.Include(ua => ua.User))
    {
        Console.WriteLine($"{ua.User.Name} {ua.RoleId}");
    }
}

void query3()
{
    // Впорядкування
    foreach(var item in dataContext.Users.OrderBy(u => u.Name))
    {
        Console.WriteLine($"{item.Name} {item.Birthdate}");
    }
    Console.WriteLine("------------------");
    foreach (var item in dataContext.Users.OrderByDescending(u => u.Birthdate))
    {
        Console.WriteLine($"{item.Name} {item.Birthdate}");
    }
}

void query4()
{
    /*
     select Users.Id, max(Users.Name), count(UserAccesses.Id) 
     from Users join UserAccesses ON Users.Id = UserAccesses.UserId
     group by Users.Id
    */
    Console.WriteLine(
        String.Join("\n",
            dataContext.Users.GroupJoin(
                dataContext.UserAccesses,
                u => u.Id,
                ua => ua.UserId,
                (u, uas) => $"{u.Name} {uas.Count()}"
            )));
    Console.WriteLine("---------------------");
    // А також за допомогою навігаційних властивостей
    Console.WriteLine(
        String.Join("\n",
        dataContext
        .Users
        .Include(u => u.UserAccesses)
        .Select(u => $"{u.Name} {u.UserAccesses.Count}")
        //.Where(str => str != String.Empty)
        //.Take(10)
        //.Skip(2)
    ));

    // foreach (User user in dataContext.Users.Include(u => u.UserAccesses))
    // {
    //     Console.WriteLine($"{user.Name} {user.UserAccesses.Count}");
    // }
}

void query5()
{
    // Які користувачі мають право на редагування даних
    // User - UserAccess - Role(canUpdate)
    // 1. SQL
    Console.WriteLine(
        String.Join('\n',
            dataContext
            .Users
            .FromSqlRaw(
                @"SELECT U.* FROM Users U
                    JOIN UserAccesses UA on U.id = UA.UserId
                    JOIN UserRoles UR on UA.RoleId = UR.Id
                    WHERE UR.CanUpdate = 1")
            .Select(u => u.Name)));
    Console.WriteLine("------------------");

    // 2. EF Join
    var q0 = dataContext
        .Users                                   // FROM Users U
        .Join(                                   // 
            dataContext.UserAccesses,            // JOIN UserAccesses UA  
            u => u.Id,                           // on U.id =
            ua => ua.UserId,                     // UA.UserId
            (u, ua) => new { u.Name, ua }        // 
        )                                        // 
        .Join(                                   // 
            dataContext.UserRoles,               // JOIN UserRoles UR on 
            uua => uua.ua.RoleId,                // UA.RoleId = 
            r => r.Id,                           // UR.Id
            (uua, r) => new { uua.Name, r }      // 
        )                                        // 
        .Where(uur => uur.r.CanUpdate)           // WHERE UR.CanUpdate = 1
        .Select(uur => uur.Name);                // 
    Console.WriteLine(q0.CreateDbCommand().CommandText);
    Console.WriteLine("------------------");
    Console.WriteLine( String.Join('\n', q0 ));
    Console.WriteLine("------------------");

    // 3. EF Navigation props
    var q = dataContext
        .Users
        .Include(u => u.UserAccesses)            // JOIN UserAccesses UA on U.id = UA.UserId
        .ThenInclude(ua => ua.UserRole)          // JOIN UserRoles UR on UA.RoleId = UR.Id
        .Where(u => u.UserAccesses.Any(ua => ua.UserRole.CanUpdate))
        .Select(u => u.Name);
    Console.WriteLine(q.CreateDbCommand().CommandText);
    Console.WriteLine("------------------");
    Console.WriteLine( String.Join('\n', q ));
    Console.WriteLine("------------------");

    // 4. EF Inverse Navigation props
    var q2 = dataContext
        .UserRoles
        .Where(ur => ur.CanUpdate)
        .Include(ur => ur.UserAccesses)
        .ThenInclude(ua => ua.User)
        .Select(ur => String.Join('\n', ur.UserAccesses.Select(ua => ua.User.Name)));
    Console.WriteLine(q2.CreateDbCommand().CommandText);
    Console.WriteLine("------------------"); 
    Console.WriteLine( String.Join('\n', q2 ));


    //Console.WriteLine(
    //    String.Join('\n',
    //));
}

void SignUp()
{
    Console.Write("Enter Name: ");
    String name = Console.ReadLine()!;
    Console.Write("Enter Email: ");
    String email = Console.ReadLine()!;
    Console.Write("Enter Login: ");
    String login = Console.ReadLine()!;
    Console.Write("Enter Password: ");
    String password = Console.ReadLine()!;

    // Процедура реєстрації - дві сутноті User, UserAccess
    // TODO: валідація даних - перевірка за типовими шаблонами

    Guid userId = Guid.NewGuid();

    User user = new() { 
        Id = userId,
        Name = name,
        Email = email,
        RegisteredAt = DateTime.Now,
    };
    String salt = Random.Shared.Next().ToString();
    UserAccess userAccess = new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Login = login,
        Salt = salt,
        Dk = kdf(password, salt),
        RoleId = "SelfRegistered"
    };
    // додаємо нові об'єкти до контексту
    dataContext.Users.Add(user);
    dataContext.UserAccesses.Add(userAccess);
    // після додавання даних до контексту вони доступні у програмі, але
    // не передані до БД
    dataContext.SaveChanges();
    Console.WriteLine("You are registered");
}

void SignIn()
{
    Console.Write("Enter Login: ");
    String login = Console.ReadLine()!;
    Console.Write("Enter Password: ");
    String password = Console.ReadLine()!;

    // шукаємо у UserAccess, але оскільки пароль "солений", спочатку шукаємо за 
    // логіном, а потім перевіряємо правильність паролю
    if (dataContext
        .UserAccesses
        .FirstOrDefault(ua => ua.Login == login)
        is UserAccess userAccess)
    {
        // пароль перевіряється шляхом розрахунку DK з солі, що збережена у БД
        // та паролю, який ввів користувач. Результат розрахунку має збігатись
        // зі збереженим у БД DK
        String dk = kdf(password, userAccess.Salt) ;
        if (dk == userAccess.Dk)
        {
            Console.WriteLine("OK");
            return;
        }
    }
    Console.WriteLine("Access denied");
}

String kdf(String password, String salt)   // By RFC 2898
{
    int c = 3;
    int dkLen = 20;
    String t = password + salt;
    for (int i = 0; i < c; i++)
    {
        t = hash(t);
    }
    return t[..dkLen]; // t.Substring(0, dkLen);
}

String hash(String input)
{
    return Convert.ToHexString(
        System.Security.Cryptography.SHA1.HashData(
            System.Text.Encoding.UTF8.GetBytes(input)
        ));
}

/* Д.З. Впровадити механізм реєстрації/автентифікації
 * з дотриманням вимог стандартів, зокрема RFC 2898
 */