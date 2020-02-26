using UnityEngine;
using UniRx;
using ZP.Lib;
using System;
using NUnit.Framework;
using ZP.Lib.CoreEx;
using ZP.Lib.CoreEx.Tools;

namespace ZP.Lib.Server.Test.Entity
{
    public interface IMainScene
    {

    }

    public class MainScene : MonoBehaviour , IMainScene
    {
        //Common Inspector's property
        public TextAsset strData;
        public Vector3 startPos;

        public bool bFlag;

        public string strString;

        //not support
        public GameObject ZUI;

        //for test 
        public bool bACallAwake = false;
        public bool bACallStart = false;
        public bool bACallDestory= false;

        public bool bACallNextFrame = false;

        public IReactiveProperty<int> taskCount = new ReactiveProperty<int>(0);

        public int CollisionEnterCount = 0;

        public int CollisionExitCount = 0;

        static public object GetObj()
        {
            return ZPropertyMesh.CreateObject<TestObj>() as object;
        }

        private void Awake()
        {
            bACallAwake = true;
        }


        Person person;
        IDisposable IDis = null;
        // Use this for initialization
        void Start()
        {
            var runId = MatrixRuntimeTools.GetRunId();
            Assert.IsTrue(string.Compare("TestRoom", runId) == 0);

            var com = gameObject.GetComponent(typeof(MainScene));
            Assert.IsTrue(com != null);

            var comI = gameObject.GetComponent(typeof(IMainScene));
            Assert.IsTrue(comI != null);

            var comT = gameObject.GetComponent<MainScene>();
            Assert.IsTrue(comT != null);

            bACallStart = true;

            TestInspectorValueBind();

            TestMathAndTransform();

            TestBindObj();

            TestMultiBindObj();

            TestCollision();

            TestUniRxEx();

        }

        int i = 100;
        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Update " + person.blood.Value.ToString());
            if (person != null)
                person.blood.Value = i++;
        }

        private void TestInspectorValueBind()
        {
            //test BindComponent
            Assert.AreEqual(startPos.x, 2.0f, 0.01f);
            Assert.AreEqual(startPos.y, 0.5f, 0.01f);
            Assert.AreEqual(startPos.z, 4.0f, 0.01f);

            Assert.IsTrue(bFlag == true);

            Assert.IsTrue(string.Compare(strString, "TestData") == 0);
        }

        private void TestMathAndTransform()
        {
            //test gameobject transform
            var main = GameObject.Find("main"); //self
            Assert.AreEqual(main.transform.position.x, 5.0f, 0.01f);
            Assert.AreEqual(main.transform.localPosition.x, 5.0f, 0.01f);

            var sub = main.transform.Find("Person.weapon");
            Assert.AreEqual(sub.transform.localPosition.x, 1.5f, 0.01f);

            //euler is (0, 180, 0)
            Assert.AreEqual(sub.transform.position.x, 3.5f, 0.01f);

            Assert.AreEqual(main.transform.forward.z, -1.0f, 0.01f);

            Assert.AreEqual(sub.transform.forward.z, 1.0f, 0.01f);

            Assert.AreEqual(main.transform.right.x, -1.0f, 0.01f);

            Assert.AreEqual(Vector3.Dot(main.transform.forward, Vector3.up), 0.0f, 0.01f);

            Assert.AreEqual(Vector3.Dot(main.transform.forward, Vector3.forward), -1.0f, 0.01f);

            Assert.AreEqual(Vector3.Dot(sub.transform.forward, Vector3.forward), 1.0f, 0.01f);

            Assert.AreEqual(main.transform.rotation.y, 1.0f, 0.01f);
            Assert.AreEqual(main.transform.localRotation.y, 1.0f, 0.01f);

            Assert.AreEqual(sub.transform.rotation.y, 0.0f, 0.01f);
            Assert.AreEqual(sub.transform.localRotation.y, 1.0f, 0.01f);

            Assert.AreEqual(main.transform.eulerAngles.x, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.eulerAngles.y, 180.0f, 0.01f);
            Assert.AreEqual(main.transform.eulerAngles.z, 0.0f, 0.01f);

            Assert.AreEqual(main.transform.localEulerAngles.x, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.localEulerAngles.y, 180.0f, 0.01f);
            Assert.AreEqual(main.transform.localEulerAngles.z, 0.0f, 0.01f);

            Assert.AreEqual(sub.transform.eulerAngles.x, 0.0f, 0.01f);
            Assert.AreEqual(sub.transform.eulerAngles.y, 0.0f, 0.01f);
            Assert.AreEqual(sub.transform.eulerAngles.z, 0.0f, 0.01f);

            Assert.AreEqual(sub.transform.localEulerAngles.x, 0.0f, 0.01f);
            Assert.AreEqual(sub.transform.localEulerAngles.y, 180.0f, 0.01f);
            Assert.AreEqual(sub.transform.localEulerAngles.z, 0.0f, 0.01f);

            //test scale
            main.transform.localScale = new Vector3(2, 2, 2);
            Assert.AreEqual(main.transform.position.x, 5.0f, 0.01f);
            Assert.AreEqual(sub.transform.position.x, 2.0f, 0.01f);


            //test for set euler
            main.transform.eulerAngles = new Vector3(0, 0, 0);
            var angleV = main.transform.localEulerAngles.AngleNormalize();
            Assert.AreEqual(angleV.x, 0.0f, 0.01f);
            Assert.AreEqual(angleV.y, 0.0f, 0.01f);
            Assert.AreEqual(angleV.z, 0.0f, 0.01f);

             angleV = sub.transform.eulerAngles.AngleNormalize();
            Assert.AreEqual(angleV.x, 0.0f, 0.01f);
            Assert.AreEqual(angleV.y, 180.0f, 0.01f);
            Assert.AreEqual(angleV.z, 0.0f, 0.01f);

            main.transform.eulerAngles = new Vector3(0, 60, 0);

            //8660254f
            Assert.AreEqual(main.transform.rotation.x, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.rotation.y, 0.5f, 0.01f);
            Assert.AreEqual(main.transform.rotation.z, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.rotation.w, 0.8660254f, 0.01f);

            main.transform.eulerAngles = new Vector3(0, 180, 0);

            //8660254f
            Assert.AreEqual(main.transform.rotation.x, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.rotation.y, 1.0f, 0.01f);
            Assert.AreEqual(main.transform.rotation.z, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.rotation.w, 0.0f, 0.01f);

            main.transform.eulerAngles = new Vector3(0, 180, 180);

            angleV = main.transform.localEulerAngles.AngleNormalize();
            Assert.AreEqual(angleV.x, 0.0f, 0.01f);
            Assert.AreEqual(angleV.y, 180.0f, 0.01f);
            Assert.AreEqual(angleV.z, 180.0f, 0.01f);

            //8660254f
            Assert.AreEqual(main.transform.rotation.x, 1.0f, 0.01f);
            Assert.AreEqual(main.transform.rotation.y, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.rotation.z, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.rotation.w, 0.0f, 0.01f);

            Assert.AreEqual(main.transform.localRotation.x, 1.0f, 0.01f);
            Assert.AreEqual(main.transform.localRotation.y, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.localRotation.z, 0.0f, 0.01f);
            Assert.AreEqual(main.transform.localRotation.w, 0.0f, 0.01f);

            main.transform.LookAt(Vector3.right, Vector3.up);

            angleV = main.transform.localEulerAngles.AngleNormalize();
            Assert.AreEqual(angleV.x, 0.0f, 0.01f);
            Assert.AreEqual(angleV.y, 90.0f, 0.01f);
            Assert.AreEqual(angleV.z, 0.0f, 0.01f);

            var bindTarget = GameObject.Find("Person"); //self

            main.transform.LookAt(bindTarget.transform, Vector3.up);

            angleV = main.transform.localEulerAngles.AngleNormalize();
            Assert.AreEqual(angleV.x, 315f, 0.01f);
            Assert.AreEqual(angleV.y, 270.0f, 0.01f);
            Assert.AreEqual(angleV.z, 0.0f, 0.01f);
        }

        private void TestBindObj()
        {
            //test bind
            person = ZPropertyMesh.CreateObject<Person>();
            person.blood.Value = 100;
            person.rank.Value = 2;

            Weapon sword = person.weapon.Value;
            sword.power.Value = 991.0f;

            person.testList.Add(900);
            person.testList.Add(100);

            var testObj = ZPropertyMesh.CreateObject<TestObj>();
            testObj.testData.Value = 201;

            person.testInterface.Value = testObj;

            var pp = ZPropertyMesh.GetPropertyEx(person, ".weapon.power");

            //ZPropertyPrefs.Save((object)sword, "");
            ZPropertyPrefs.LoadFromStr(person, strData.text);

            person.blood2.Upgrade(1);

            Assert.IsTrue(person.blood2.Value == 109); //level 1   [99, 109, 119]

            Assert.IsTrue(person.blood.Value == 90);

            Assert.IsTrue(person.BMale);

            Assert.IsTrue(person.testList.Count == 3);

            Assert.AreEqual(person.pos.Value.x, 1.0);

            Assert.IsTrue(person.degree.Value == DegreeEnum.UniversityDegree);

            Assert.AreEqual(person.testInterface.Value.TestData, 1.225, 0.01f);

            Debug.Log("TestProperty Start  perion's l " + person.weapon.Value.power.Value);

            //transform.Find()
            var bindTarget = GameObject.Find("Person"); //self

            Assert.IsTrue(bindTarget != null);

            //only support Server
            //bindTarget.transform.enabled = true;

            var bBind = ZViewBuildTools.BindObject(person, bindTarget.transform);

            Assert.IsTrue(bBind == true);

            // 3 + 1 (Weapon.power) 
            Assert.IsTrue(bindTarget.GetAllChildren().Count == 5);

            Assert.IsTrue(bindTarget.GetChildren().Count == 4);

            Assert.IsTrue(bindTarget.transform.childCount == 4);

            //bindTarget.transform.GetChild()

            var testInter = bindTarget.transform.Find("Person.testInterface2");
            Assert.IsTrue(testInter != null);

            var testPosObj = bindTarget.transform.Find("Person.pos");
            Assert.AreEqual(testPosObj.position.x, 1.0f, 0.01f);
            Assert.AreEqual(testPosObj.position.y, 2.0f, 0.01f);
            Assert.AreEqual(testPosObj.position.z, 1.5f, 0.01f);

            person.pos.Value = new Vector3(3, 3, 3.5f);
            Assert.AreEqual(testPosObj.position.x, 3.0f, 0.01f);
            Assert.AreEqual(testPosObj.position.y, 3.0f, 0.01f);
            Assert.AreEqual(testPosObj.position.z, 3.5f, 0.01f);

            var weaponPosObj = bindTarget.transform.Find("Person.weapon");
            Assert.AreEqual(weaponPosObj.position.x, 0.0f, 0.01f);
            Assert.AreEqual(weaponPosObj.position.y, 2.0f, 0.01f);
            Assert.AreEqual(weaponPosObj.position.z, 3.0f, 0.01f);

            person.weapon.Value.transform.Value.Position.Value = new Vector3(3, 3, 3.5f);
            Assert.AreEqual(weaponPosObj.position.x, 3.0f, 0.01f);
            Assert.AreEqual(weaponPosObj.position.y, 3.0f, 0.01f);
            Assert.AreEqual(weaponPosObj.position.z, 3.5f, 0.01f);
            

            //test multi add component bind
            var rtObj = weaponPosObj.GetComponent<RTWeaponObj>();

            Assert.IsTrue(rtObj != null);
            Assert.IsTrue(rtObj.bBind == true);

           var rtObj2 = weaponPosObj.GetComponent<RTWeaponObj2>();

            Assert.IsTrue(rtObj2 != null);
            Assert.IsTrue(rtObj2.bBind == true);

            var rtObj3 = weaponPosObj.GetComponent<RTWeaponObj3>();

            Assert.IsTrue(rtObj3 != null);
            Assert.IsTrue(rtObj3.bBind == true);

            Assert.IsTrue(person.weapon.TransNode != null);
            Assert.IsTrue(person.weapon.Value.power.TransNode != null);

            ZViewBuildTools.UnBindObject(person, bindTarget.transform);

            Assert.IsTrue(rtObj.bBind == false);
            Assert.IsTrue(rtObj2.bBind == false);
            Assert.IsTrue(rtObj3.bBind == false);

            Assert.IsTrue(person.weapon.TransNode == null);
            Assert.IsTrue(person.weapon.Value.power.TransNode == null);

            //test for bind Group
            ZViewBuildTools.BindObject(person, bindTarget.transform, "Group1");
            testPosObj = bindTarget.transform.Find("Person.pos");
            weaponPosObj = bindTarget.transform.Find("Person.weapon");

            Assert.IsTrue(person.pos.TransNode != null);
            Assert.IsTrue(person.weapon.TransNode == null);

            //reset the pos
            //person.pos.Value = new Vector3(0, 5, 0);
            //Assert.AreEqual(testPosObj.position.x, 0.0f, 0.01f);
            //Assert.AreEqual(testPosObj.position.y, 5.0f, 0.01f);
            //Assert.AreEqual(testPosObj.position.z, 0.0f, 0.01f);

            ZViewBuildTools.UnBindObject(person, bindTarget.transform);
        }

        private void TestMultiBindObj()
        {
            var bindTarget = GameObject.Find("Person2");

            ZViewBuildTools.BindObject(person, bindTarget.transform);

            Assert.IsTrue(person.rank.TransNode != null);
            Assert.IsTrue(person.weapon.TransNode != null);
            Assert.IsTrue(person.weapon.Value.power.TransNode != null);

            ZViewBuildTools.UnBindObject(person, bindTarget.transform);
        }

        private void TestCollision()
        {
            var bindTarget = GameObject.Find("Person"); //self

            //include self's components
            var boxs = bindTarget.GetComponentsInChildren<BoxCollider>();

            //test bounds two box ,but slef's box is index 0
            var box = boxs[0];

            Assert.AreEqual(box.bounds.center.x, 0.0f, 0.01f);
            Assert.AreEqual(box.bounds.center.y, 5.0f, 0.01f);
            Assert.AreEqual(box.bounds.center.z, 0.0f, 0.01f);

            Assert.AreEqual(box.bounds.size.x, 1.0f, 0.01f);
            Assert.AreEqual(box.bounds.size.y, 1.0f, 0.01f);
            Assert.AreEqual(box.bounds.size.z, 1.0f, 0.01f);

            Assert.IsTrue(box.bounds.Contains(new Vector3(0, 4.9f, 0)));
            Assert.IsTrue(box.bounds.Contains(new Vector3(0, 0.49f, 0)) == false);

            Assert.IsTrue(box.bounds.Contains(new Vector3(0, 5.6f, 0)) == false);

            Assert.IsTrue(boxs.Count == 3); //weapon's box and person's box

            //test OverlapBox

            box.enabled = false;

            boxs = bindTarget.GetComponentsInChildren<BoxCollider>();
            var cols = bindTarget.GetComponentsInChildren<Collider>();
            //if component is disabled, but we can find it
            Assert.IsTrue(boxs.Count == 3);
            Assert.IsTrue(cols.Count == 3);

            //send a weapon 
            var weaponPosObj = bindTarget.transform.Find("Person.weapon");

            weaponPosObj.position = new Vector3(5, 0, 0);

            var weaponBox = weaponPosObj.GetComponentInChildren<BoxCollider>();
            Assert.IsTrue(weaponBox.enabled == false);
            weaponBox.enabled = true;
            box.enabled = true; //enable and will invoke the collision check


        }

        void TestUniRxEx()
        {
            //IReactiveProperty<bool> taskEnd = new ReactiveProperty<bool>(false);

            var disp = ObservableEx.NextFrame().ObserveOn(Scheduler.CurrentThread).Subscribe(_ =>
                {
                    //Debug.Log("ObservableEx.NextFrame")
                    //taskEnd.Value = true;
                    var runId = MatrixRuntimeTools.GetRunId();
                    Assert.IsTrue(string.Compare("TestRoom", runId) == 0);

                    bACallNextFrame = true;
                });

            //taskEnd.Where(cur => cur == true).Fetch().Timeout(TimeSpan.FromSeconds(5)). ToTask().Wait();

            //disp.Dispose();
            //taskEnd.Value = false;


            
            disp = ObservableEx.EveryUpdate().ObserveOn(Scheduler.CurrentThread).Subscribe(_ =>
            {
                var runId = MatrixRuntimeTools.GetRunId();
                Assert.IsTrue(string.Compare("TestRoom", runId) == 0);
                //Debug.Log("ObservableEx.NextFrame")
                taskCount.Value++;
            });

            //taskCount.Where(cur => cur > 10).Fetch().Timeout(TimeSpan.FromSeconds(5)).ToTask().Wait();

            Observable.Timer(new TimeSpan(0, 0, 1)).ObserveOn(ZPRxScheduler.CurrentScheduler).Subscribe(_ => {
                var runId = MatrixRuntimeTools.GetRunId();
                Assert.IsTrue(string.Compare("TestRoom", runId) == 0);

            });
        }

        private void OnCollisionEnter(Collision collision)
        {
            var runId = MatrixRuntimeTools.GetRunId();
            Assert.IsTrue(string.Compare("TestRoom", runId) == 0);

            Debug.Log("OnCollisionEnter " + collision.collider.name);

            Assert.IsTrue(string.Compare(collision.collider.name, "Person.weapon") == 0);

            //[TODO]
            //Assert.IsTrue(collision.contacts != null);

            GameObject.Destroy(collision.gameObject);

            CollisionEnterCount++;
        }

        private void OnCollisionExit(Collision collision)
        {
            Debug.Log("OnCollisionExit " + collision.collider.name);

            CollisionExitCount++;
        }

        void OnDestroy()
        {
            bACallDestory = true;
        }

        public void Stop()
        {
            IDis?.Dispose();
        }

    }

}
