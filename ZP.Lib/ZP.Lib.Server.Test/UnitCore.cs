using NUnit.Framework;
using ZP.Lib;
using ZP.Lib.Server.Test.Entity;
using UniRx;
using System.Threading;
using System;
using System.Threading.Tasks;
using ZP.Lib.CoreEx.Reactive;
using ZP.Lib.Core.Domain;
using ZP.Lib.Core.Main;
using System.Collections.Generic;

namespace ZP.Lib.Server.Test
{
    
    public class TestCore
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestPropObjectSingleton()
        {
            TestObjectSingletonWIthId.GetInstance("test").IntProp.Value = 100;

            Assert.IsTrue(TestObjectSingletonWIthId.GetInstance("test").IntProp.Value == 100);

            //TestObjectSingletonWIthIdSub.GetInstance< TestObjectSingletonWIthIdSub>().

            var testobj = PropObjectSinglenode<TestObjectSingle>.Instance;
            testobj.IntProp.Value = 200;
            Assert.IsTrue(PropObjectSinglenode<TestObjectSingle>.Instance.IntProp.Value == 200);


            List<TestObjectSingleWithTaskScheduler> testTaskList = new List<TestObjectSingleWithTaskScheduler>();

            //int threadId = -1;
            IReactiveProperty<int> taskCount = new ReactiveProperty<int>(0);
            var result =  Parallel.For(0, 5, _ =>
            {
                Task a = new Task(() =>
                {
                    Thread.Sleep(100);
                    var threadId = TaskScheduler.Current.Id;
                    var testTaskObjInner = (TestObjectSingleWithTaskScheduler.Instance);

                    testTaskObjInner.IntProp.Value = 99 + threadId;
                    lock (typeof(TestObjectSingleWithTaskScheduler))
                    {
                        testTaskList.Add(testTaskObjInner);
                        taskCount.Value++;
                    }
                });
                a.Start(TaskScheduler.Default);
            });

            taskCount.Where(cur => cur == 5).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();

            //Assert.IsTrue(threadId != -1);

            var testTaskObj = testTaskList[0];
            var testTaskObj2 = testTaskList[2];

            //Assert.IsTrue(testTaskObj.IntProp.Value != testTaskObj2.IntProp.Value);
           
            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);
            testTaskObj.RunInCurrentTaskScheduler(() =>
            {
                Assert.IsTrue(testTaskObj.IntProp.Value == TaskScheduler.Current.Id + 99);
                taskEnd.Value = true;
            });

            taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            
            TestObjectSingleWithTaskScheduler.ReleaseAll();
            TestObjectSingleWithTaskScheduler.CheckMapCount(0);

            TestObjectSingleWithTaskScheduler.Instance.IntProp.Value = 200;

            TestObjectSingleWithTaskScheduler.CheckMapCount(1);
            TestObjectSingleWithTaskScheduler.Release();

            TestObjectSingleWithTaskScheduler.CheckMapCount(0);
            Assert.IsTrue(testTaskObj != null);

           Assert.IsTrue(TestObjectSingleWithTaskScheduler.bReleased);

            Assert.IsTrue(TestObjectSingleWithTaskScheduler.Instance.IntProp.Value != 200);

            testobj = PropObjectSinglenodeWithTaskScheduler<TestObjectSingle>.Instance;
            testobj.IntProp.Value = 200;
            Assert.IsTrue(PropObjectSinglenode<TestObjectSingle>.Instance.IntProp.Value == 200);
        }

        [Test]
        public void TestProperty()
        {
            //Test Common Porperty
            var person = ZPropertyMesh.CreateObject<Person>();
            person.blood.Value = 100;
            person.rank.Value = 2;

            Assert.IsTrue(person.rank.Value == 2);
            Assert.IsTrue(person.blood.Value == 100);


            Weapon sword = person.weapon.Value;
            sword.power.Value = 991.0f;
            Assert.AreEqual(sword.power.Value, 991.0f, 0.001f);

            //test list functions
            person.testList.Add(900);
            person.testList.Add(100);

            Assert.IsTrue(person.testList.Count == 2);

           var findItem =  person.testList.FindValue(item => item == 100);
            Assert.IsTrue(findItem == 100);

            var findItems = person.testList.FindValues(item => item >= 0);
            Assert.IsTrue(findItems.Count == 2);

            var testObj = ZPropertyMesh.CreateObject<TestObj>();
            testObj.testData.Value = 201;


            //test Access
            person.testInterface.Value = testObj;
            var pp = ZPropertyMesh.GetPropertyEx(person, ".weapon.power");

            Assert.IsTrue(pp != null && (float) pp.Value > 990.0f);
            //Assert.AreEqual(pp.Value, 991.0f);

            //System.Reactive can't by used
            // Observable.Timer(new System.TimeSpan(0, 0, 1)).Repeat().Subscribe(_ => person.testList.Add(910));

            //Test PropertyDescription Attribute
            Assert.IsTrue(string.Compare(person.blood.AttributeNode.Name, "blood") == 0);
            Assert.IsTrue(string.Compare(person.blood.AttributeNode.Description, "bloodDes") == 0);

            //Assert.Pass();
        }




        static volatile int AddItemAsObservableCount = 0;
        [Test]
        public void TestListObservable()
        {
            IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);
            var person = ZPropertyMesh.CreateObject<Person>();

            person.testList.AddItemAsObservable<int>().Subscribe(_ =>
            {
                Interlocked.Increment(ref AddItemAsObservableCount);
                taskEnd.Value = true;
            });

            person.testList.DeleteItemAsObservable().Subscribe(_ =>
            {
                //AddItemAsObservableCount--;
                Interlocked.Decrement(ref AddItemAsObservableCount);
                taskEnd.Value = true;
            });

            //test list functions
            person.testList.Add(900);
            person.testList.Add(100);

            //Thread.Sleep(100);
            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();

            Assert.IsTrue(AddItemAsObservableCount == 2);

            taskEnd.Value = false;
            person.testList.Remove(item => item == 100);
            person.testList.Remove(item => item == 900);

            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
            //Thread.Sleep(100);
            Assert.IsTrue(AddItemAsObservableCount == 0);

            taskEnd.Value = false;
            person.testList.Add(900);
            person.testList.Add(100);
            //Thread.Sleep(100);
            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
            Assert.IsTrue(AddItemAsObservableCount == 2);

            taskEnd.Value = false;
            person.testList.RemoveAll(item => item > 0);
            //Thread.Sleep(100);
            taskEnd.Where(cur => cur == true).Fetch().ToTask().Wait();
            Assert.IsTrue(person.testList.Count == 0);
            Assert.IsTrue(AddItemAsObservableCount == 0);
        }

        [Test]
        public void TestPropertyCache()
        {
            var p = ZPropertyCache<Person>.Cache;

            p.blood.Value = 188;

            var p2 = ZPropertyCache<Person>.Cache;

            Assert.IsTrue(p.blood.Value == p2.blood.Value);
        }

        [Test]
        public void TestAssessQuery()
        {
            var person = ZPropertyMesh.CreateObject<Person>();

            ZPropertyPrefs.Load(person, "../../../Assets/TestPerson.json");
            
            var bloodProp = ZPropertyMesh.GetProperty(person, "Person.blood");
            Assert.IsTrue((int)bloodProp.Value == 90);

            var bloodPropValue = ZPropertyMesh.GetProperty<int>(person, "Person.blood");
            Assert.IsTrue(bloodPropValue == 90);

            bloodProp = ZPropertyMesh.GetProperty(person, "Person.blood3");
            Assert.IsTrue(bloodProp == null);

            var weapon = ZPropertyMesh.GetProperty(person, "Person.weapon");
            var powerProp = ZPropertyMesh.GetProperty(weapon, "Weapon.power");
            Assert.AreEqual((float)powerProp.Value, 12.5, 0.01f);
            Assert.AreEqual(ZPropertyMesh.GetProperty<float>(weapon, "Weapon.power"), 12.5, 0.01f);

            var powerPropT =  ZPropertyMesh.GetPropertyInSubs(person, "Weapon.power") as IZProperty<float>;
            Assert.AreEqual(powerPropT.Value, 12.5, 0.01f);

            bloodProp = ZPropertyMesh.GetPropertyEx(person, ".blood");
            Assert.IsTrue((int)bloodProp.Value == 90);

            var powerValue = ZPropertyMesh.GetPropertyEx<float>(person, ".weapon.power");
            Assert.AreEqual(powerValue, 12.5, 0.01f);


            //test for properties
            var pCount = ZPropertyMesh.GetProperties(person).Count;

            Assert.IsTrue(pCount == 10);

            //only return the Weapon's object
            pCount = ZPropertyMesh.GetPropertiesWithPropertyable(person).Count;
            Assert.IsTrue(pCount == 1);

            pCount = ZPropertyMesh.GetPropertiesWithRankable(person).Count;
            Assert.IsTrue(pCount == 1);

            pCount = ZPropertyMesh.GetPropertiesInSubs(person).Count;

            //incloud child's property ex. Weapon.power
            Assert.IsTrue(pCount == 14);

        }

        [Test]
        public void TestAssessQueryPerformance()
        {
            var person = ZPropertyMesh.CreateObject<Person>();
            person.blood.Value = 100;
            person.rank.Value = 2;

            for (int i =0; i < 10000; i++)
            {
                var bloodProp = ZPropertyMesh.GetProperty(person, "Person.blood");
            }
        }


        [Test]
        public void TestAssessPrefs()
        {
            var interfaceType = Type.GetType("ZP.Lib.Server.Test.Entity.TestObj");

            var person = ZPropertyMesh.CreateObject<Person>();

            ZPropertyPrefs.Load(person, "../../../Assets/TestPerson.json");

            person.blood2.Upgrade(1);

            Assert.IsTrue(person.blood2.Value == 109); //level 1   [99, 109, 119]

            //[TODO] Test OnLoad / OnCopy / OnCreate With DynamicProxy

            Assert.IsTrue(person.blood.Value == 90);

            Assert.IsTrue(person.BMale);

            Assert.IsTrue(person.testList.Count == 3);

            Assert.AreEqual(person.pos.Value.x, 1.0);

            Assert.IsTrue(person.degree.Value == DegreeEnum.UniversityDegree);

            Assert.AreEqual(person.testInterface.Value.TestData, 1.225, 0.01f);

            //test save
            person.blood.Value = 99;
            ZPropertyPrefs.Save(person, "../../../Assets/TestPersonCopy.json");

            var person2 = ZPropertyMesh.CreateObject<Person>();
            ZPropertyPrefs.Load(person2, "../../../Assets/TestPersonCopy.json");

            Assert.IsTrue(person2.blood.Value == 99);

            Assert.IsTrue(person2.testList.Count == 3);

            Assert.AreEqual(person2.pos.Value.x, 1.0, 0.01f);

            Assert.IsTrue(person2.degree.Value == DegreeEnum.UniversityDegree);

            Assert.IsTrue(person2.testInterface.Value == null);
        }

        [Test]
        public void TestParallelPerformance()
        {
            int innerCount = 0;
            IReactiveProperty<int> count = new ReactiveProperty<int>(0);
            Parallel.For(1, 10000, _ =>
            {
                var person = ZPropertyMesh.CreateObject<Person>();

                //Interlocked.Increment(ref count.Value);
                lock(typeof(TestCore))
                    count.Value++;

                innerCount++;
            });


            count.Where(cur => cur >= 9999).Fetch().ToTask().Wait();
            Assert.IsTrue(innerCount <= 10000);
        }

        [Test]
        public void TestPerformance()
        {
            for (int i = 0; i < 10000; i++)
            {
                var person = ZPropertyMesh.CreateObject<Person>();
            }
        }

        [Test]
        public void TestLinkAndRef()
        {
            //test link Ref bindRef

            var personLink = ZPropertyMesh.CreateObject<PersonLink>();

            Assert.IsTrue(personLink.blood3.Value == 90);

            Assert.IsTrue(personLink.zListCursor.Value == 0);

            Assert.IsTrue(personLink.zListCursor.PropCount() == 3);

            personLink.PersonRef.Value.testList.Add(900);

            Assert.IsTrue(personLink.zListCursor.PropCount() == 4);

            personLink.zListCursor.Value = 10;

            Assert.IsTrue(personLink.zListCursor.Value == 2);

            IReactiveProperty<bool> callSync = new ReactiveProperty<bool>(false);

            var cursorDispose = personLink.zListCursor.ValueChangeAsObservable<int>().Subscribe(cur =>
            {
                Assert.IsTrue(cur == 1);
                callSync.Value = true;
            });

            personLink.PersonRef.Value.testList.RemoveAt(1);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            Assert.IsTrue(personLink.zListCursor.Value == 1);

            cursorDispose.Dispose();

            callSync.Value = false;
            var farwordDispose =  personLink.blood3.ValueChangeAsObservable<int>().Subscribe(cur =>
            {
                Assert.IsTrue(cur == 100);
                callSync.Value = true;
            });

            var bloodProp = ZPropertyMesh.GetPropertyEx(personLink, ".PersonRef.blood") as IZProperty<int>;
            Assert.IsTrue((int)bloodProp.Value == 90);

            bloodProp.Value = 100;
            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();

            //stop to listen
            farwordDispose.Dispose();


            //check bidirection sync
            callSync.Value = false;
            bloodProp.ValueChangeAsObservable<int>().Subscribe(cur =>
            {
                Assert.IsTrue(cur == 200);
                callSync.Value = true;
            });

            personLink.blood3.Value = 200;

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(2)).ToTask().Wait();



        }

        [Test]
        public void TestRelation()
        {
            //select sum count
            var personLink = ZPropertyMesh.CreateObject<PersonLink>();

            Assert.IsTrue(personLink.blood3.Value == 90);

           var dispDead = personLink.bDead.Select(personLink.blood3, b => b <= 50);

            Assert.IsTrue(personLink.bDead.Value == false);


            IReactiveProperty<bool> callSync = new ReactiveProperty<bool>(false);


            personLink.bDead.ValueChangeAsObservable().Where(b => b == true).Subscribe(b =>
            {
                //is dead
                Assert.IsTrue(b);
                callSync.Value = true;
            });

            personLink.blood3.Value = 40;

            callSync.Where(cur => cur == true).Fetch().Timeout( TimeSpan.FromSeconds(5)).ToTask().Wait();

            dispDead.Dispose();
            callSync.Value = false;

            personLink.bDead.ValueChangeAsObservable().Subscribe(b =>
            {
                //is dead
                Assert.IsTrue(b);
                callSync.Value = true;
            });

            //for test dispose
            personLink.blood3.Value = 100;

            //for Count
            var dispCountLink = personLink.testCount.Count(personLink.PersonRef.Value.testList);

            Assert.IsTrue(personLink.testCount.Value == 3);

            var dispCount = personLink.testCount.ValueChangeAsObservable().Subscribe(c =>
            {
                //is dead
                Assert.IsTrue(c == 4);
                callSync.Value = true;
            });
            personLink.PersonRef.Value.testList.Add(199);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();

            Assert.IsTrue(personLink.testCount.Value == 4);

            dispCount.Dispose();
            dispCountLink.Dispose();
            callSync.Value = false;

            //relink
            dispCountLink = personLink.testCount.Count(personLink.PersonRef.Value.testList, a => a > 20);

            Assert.IsTrue(personLink.testCount.Value == 2);

            dispCount = personLink.testCount.ValueChangeAsObservable().Subscribe(c =>
            {
                //is dead
                Assert.IsTrue(c == 1);
                callSync.Value = true;
            });

            personLink.PersonRef.Value.testList.RemoveAt(3);
            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            Assert.IsTrue(personLink.testCount.Value == 1);

            //for Sum

            var perView = ZPropertyMesh.CreateObject<PersonView>();

            for (int i = 0; i < 10; i++)
            {
                var pL = ZPropertyMesh.CreateObject<PersonLink>();

                pL.PersonRef.Value.blood.Value = 90;
                perView.personLinks.Add(pL);
            }

           var dispSum =  perView.totalBlood.Sum(perView.personLinks, ".PersonRef.blood");
            perView.totalfloatBlood.Sum(perView.personLinks, ".PersonRef.blood");

            Assert.IsTrue(perView.totalBlood.Value == 900);
            Assert.AreEqual(perView.totalfloatBlood.Value, 900.0, 0.1f);
            callSync.Value = false;

            var dispValueChange =  perView.totalBlood.ValueChangeAsObservable().Subscribe(cur =>
            {
                Assert.IsTrue(perView.totalBlood.Value == 990);
                Assert.IsTrue(cur == 990);
                callSync.Value = true;
            });

            var pLNew = ZPropertyMesh.CreateObject<PersonLink>();

            pLNew.PersonRef.Value.blood.Value = 90;
            perView.personLinks.Add(pLNew);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();

            Assert.IsTrue(perView.totalBlood.Value == 990);
            callSync.Value = false;

            dispSum.Dispose();

            perView.personLinks.RemoveAt(0);
            Assert.IsTrue(perView.personLinks.Count == 10);

            Thread.Sleep(400);

            dispValueChange.Dispose();

            //reset the view
            dispSum = perView.totalBlood.Sum(perView.personLinks, ".PersonRef.blood");

            Assert.IsTrue(perView.totalBlood.Value == 900);

            dispValueChange =  perView.totalBlood.ValueChangeAsObservable().Subscribe(cur =>
            {
                Assert.IsTrue(perView.totalBlood.Value == 810);
                Assert.IsTrue(cur == 810);
                callSync.Value = true;
            });

            perView.personLinks.RemoveAt(0);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();


            dispSum.Dispose();
            dispValueChange.Dispose();
            callSync.Value = false;

            perView.personLinks.ClearAll();

            //test ZpropertyRefList sum
            for (int i = 0; i < 10; i++)
            {
                var pL = ZPropertyMesh.CreateObject<PersonLink>();

                pL.PersonRef.Value.blood.Value = i + 1;
                perView.personRefLinks.Add(i, pL);
            }

            dispSum = perView.totalBlood.Sum(perView.personRefLinks, ".PersonRef.blood");

            Assert.IsTrue(perView.totalBlood.Value == 55);

            dispValueChange = perView.totalBlood.ValueChangeAsObservable().Subscribe(cur =>
            {
                Assert.IsTrue(perView.totalBlood.Value == 75);
                Assert.IsTrue(cur == 75);
                callSync.Value = true;
            });

            pLNew = ZPropertyMesh.CreateObject<PersonLink>();

            pLNew.PersonRef.Value.blood.Value = 20;
            perView.personRefLinks.Add(10, pLNew);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();

            //dispSum.Dispose();
            dispValueChange.Dispose();
            callSync.Value = false;

            dispValueChange = perView.totalBlood.ValueChangeAsObservable().Subscribe(cur =>
            {
                Assert.IsTrue(perView.totalBlood.Value == 65);
                Assert.IsTrue(cur == 65);
                callSync.Value = true;
            });

            perView.personRefLinks.Remove(pl => pl.PersonRef.Value.blood.Value == 10);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();


        }


        [Test]
        public void TestRefListRelation()
        {
            // Select Where Cache Sort Merge
            var perView = ZPropertyMesh.CreateObject<PersonView>();

            for (int i = 0; i < 10; i++)
            {
                var pL = ZPropertyMesh.CreateObject<PersonLink>();
                pL.linkId.Value = i;
                pL.PersonRef.Value.blood.Value = i;
                perView.personLinks.Add(pL);

                //is indexer
                perView.personRefLinks.Add(pL);

                perView.personLinks2.Add(pL);
            }

            var dispRefLink = perView.personRefLinksView.Select(perView.personLinks, pl => pl.PersonRef.Value);

            Assert.IsTrue(perView.personRefLinksView.Count == 10);

            var p = perView.personRefLinksView.FindValue(v => v == 0);
            Assert.IsTrue(p != null);

            perView.personLinks.RemoveAt(0);

            Assert.IsTrue(perView.personRefLinksView.Count == 9);

            p = perView.personRefLinksView.FindValue(v => v == 0);
            Assert.IsTrue(p == null);

            dispRefLink.Dispose();

            perView.personLinks.RemoveAt(0);

            //dis link so count will not change
            Assert.IsTrue(perView.personLinks.Count == 8);
            Assert.IsTrue(perView.personRefLinksView.Count == 9);

            //test for link to reflist
            dispRefLink = perView.personRefLinksView.Select(perView.personRefLinks, pl => pl.Value.PersonRef.Value);
            Assert.IsTrue(perView.personRefLinksView.Count == 19);

            IReactiveProperty<bool> callSync = new ReactiveProperty<bool>(false);

           var dispAddItem =  perView.personRefLinksView.AddItemAsObservable<Person>().Subscribe(v =>
            {
                Assert.IsTrue(v.blood.Value == 21122);
                //if (true)
                //    throw new Exception("Not Add the list");
            });

            var dispDelItem = perView.personRefLinksView.DeleteItemAsObservable<Person>().Subscribe(v =>
            {
                Assert.IsTrue(v.blood.Value == 2);
                callSync.Value = true;
            });

            perView.personRefLinks.RemoveAt(2);

            Assert.IsTrue(perView.personRefLinksView.Count == 18);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            dispAddItem.Dispose();

            dispDelItem.Dispose();

            //test for dispose
            perView.personRefLinks.RemoveAt(1);

            //test for where
            dispRefLink =  perView.personLinkRefLinksView2.Where(perView.personRefLinks, item => item.Value.blood3.Value > 5);
            Assert.IsTrue(perView.personLinkRefLinksView2.Count == 4);

            perView.personRefLinks.RemoveAt(6);

            Thread.Sleep(200);

            Assert.IsTrue(perView.personLinkRefLinksView2.Count == 3);
            dispRefLink.Dispose();

            perView.personRefLinks.RemoveAt(6);

            Assert.IsTrue(perView.personLinkRefLinksView2.Count == 3);

            perView.personLinkRefLinksView2.ClearAll();
            Assert.IsTrue(perView.personLinkRefLinksView2.Count == 0);
            callSync.Value = false;

            //test cache [TODO]
            dispRefLink = perView.personLinkRefLinksView2.Cache(perView.personRefLinks);
            Assert.IsTrue(perView.personLinkRefLinksView2.Count == 6);

            dispAddItem = perView.personLinkRefLinksView2.AddItemAsObservable<PersonLink>().Subscribe(v =>
            {
                Assert.IsTrue(v.blood3.Value == 10);
                callSync.Value = true;
            });

            var pLNew = ZPropertyMesh.CreateObject<PersonLink>();
            pLNew.linkId.Value = 10;
            pLNew.PersonRef.Value.blood.Value = 10;
            perView.personRefLinks.Add(pLNew);

            callSync.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();
            callSync.Value = false;

            Assert.IsTrue(perView.personLinkRefLinksView2.Count == 7);

            var dispSum = perView.totalBlood.Sum(perView.personLinkRefLinksView2, ".PersonRef.blood");

            Assert.IsTrue(perView.totalBlood.Value == 35);
            
            dispAddItem.Dispose();

            pLNew = ZPropertyMesh.CreateObject<PersonLink>();
            pLNew.linkId.Value = 11;
            pLNew.PersonRef.Value.blood.Value = 11;
            perView.personRefLinks.Add(pLNew);

            Assert.IsTrue(perView.totalBlood.Value == 46);
            dispRefLink.Dispose();
            dispSum.Dispose();

            perView.personLinkRefLinksView2.ClearAll();

            dispRefLink = perView.personLinkRefLinksView2.
                Merge<PersonLink>(perView.personLinks, perView.personLinks2, null);

            // remove 2 times
            Assert.IsTrue(perView.personLinkRefLinksView2.Count == 18);

        }
    }
}