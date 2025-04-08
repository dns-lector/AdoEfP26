using AdoEfP26.Data;
using AdoEfP26.Data.Entities;
using Microsoft.EntityFrameworkCore;

DataContext dataContext = new();
query5();


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


/* Д.З. Скласти EF-запити та вивести результати для наступних задач:
 * - Вивести найстаршого користувача (за віком)
 * - Вивести трьох останніх зареєстрованих користувачів (за датою реєстрації)
 * - Вивести назву ролі -- кількість користувачів, що мають цю роль
 * до звіту додавати скріншоти з результатами
 */