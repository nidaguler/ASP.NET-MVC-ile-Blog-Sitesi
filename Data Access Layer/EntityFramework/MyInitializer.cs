using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using NeYemekYapsam.Entities;

namespace NeYemekYapsam.DataAccessLayer.EntityFramework
{
    public class MyInitializer: CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {

            //adding admin user...
            NeYemekYapsamUser admin = new NeYemekYapsamUser()
            {
                Name = "walden",
                Surname = "walden",
                Email = "plevrut@gmail.com",
                Hakkımda="Merhaba ben yüzüyorum.",
                Twitter="walden",
                Facebook="walden",
                Instagram="walden",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "walden",
                Password = "123456",
                ProfileImageFilename = "user.png",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "walden",
                Meslek ="Dansçı",
                Yetenekler="Gitar, Mızıka",
                //Followings = 0,
                //Followers = 0


            };

            //adding standard user..
            NeYemekYapsamUser standardUser = new NeYemekYapsamUser()
            {
                Name = "walden",
                Surname = "walden",
                Email = "plevrut@gmail.com",
                Hakkımda = "Merhaba ben koşuyorum.",
                Twitter = "nedlaw",
                Facebook = "nedlaw",
                Instagram = "nedlaw",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Username = "nedlaw",
                Password = "654321",
                ProfileImageFilename = "user.png",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(65),
                ModifiedUsername = "walden",
                Meslek = "Sokak hayvanları koruyucusu",
                Yetenekler = "Mama Dağıtımı",
                //Followings = 0,
                //Followers = 0

            };
            context.NeYemekYapsamUsers.Add(admin);
            context.NeYemekYapsamUsers.Add(standardUser);

            for (int i = 0; i < 8; i++)
            {
                NeYemekYapsamUser user = new NeYemekYapsamUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    Hakkımda = FakeData.TextData.GetSentence(),
                    Twitter = FakeData.NetworkData.GetMacAddress(),
                    Facebook = FakeData.NetworkData.GetMacAddress(),
                    Instagram = FakeData.NetworkData.GetMacAddress(),
                    ProfileImageFilename = "user.png",
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    Username = $"user{i}",
                    Password = "123",
                    //Followings = 0,
                    //Followers = 0,
                    Meslek ="Sokak hayvanları koruyucusu",
                    Yetenekler="Mama dağıtımı",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsername = $"user{i}"

                };
                context.NeYemekYapsamUsers.Add(user);
            }





            context.SaveChanges();

            //User list for using..

            List<NeYemekYapsamUser> userlist = context.NeYemekYapsamUsers.ToList();

            // adding fake categories
            for (int i = 0; i < 10; i++)
            {
                Category cat = new Category()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    Description = FakeData.PlaceData.GetAddress(),
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ModifiedUsername = "walden",

                };

                context.Categories.Add(cat);

                //adding fake notes...
                for (int k = 0; k < FakeData.NumberData.GetNumber(5,9); k++)
                {
                    NeYemekYapsamUser owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];

                    Note note = new Note()
                    {
                        Title=FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5,25)),
                        Text=FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1,3)),
                     
                        IsDraft=false,
                        LikeCount=FakeData.NumberData.GetNumber(1,9),
                        Owner=owner,
                        CreatedOn=FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1),DateTime.Now),
                        ModifiedOn= FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername= owner.Username,
                    };
                    cat.Notes.Add(note);

                    //adding fake comments...
                    for (int j = 0; j < FakeData.NumberData.GetNumber(3,5); j++)
                    {
                        NeYemekYapsamUser comment_owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];

                        Comment comment = new Comment()
                        {
                            Text =FakeData.TextData.GetSentence(),
                           
                            Owner = comment_owner,
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsername = comment_owner.Username
                        };
                        note.Comments.Add(comment);
                    }

                    //adding fake likes...
                    
                    for (int m = 0; m <note.LikeCount; m++)
                    {
                        Liked liked = new Liked()
                        {
                            LikedUser = userlist[m]
                        };

                        note.Likes.Add(liked);
                    }



                }

            }
            context.SaveChanges();
        }
    }
}
