using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DTO;

namespace CodeArtTest.DTO
{
    /// <summary>
    /// DTOTest 的摘要说明
    /// </summary>
    [TestClass]
    public class DTOAdvancedTest
    {
        public DTOAdvancedTest()
        {

        }

        private const string _code0 = "{\"name\":\"名称\",\"orderIndex\":\"1\",\"markedCode\":\"markedCode\",\"description\":\"这是一项描述\",\"attached\":[{\"name\":\"配置1\",\"type\":\"3\",\"required\":\"true\",\"options\":\"选项1\"},{\"name\":\"配置2\",\"type\":\"2\",\"required\":\"true\",\"options\":\"选项1，选项2\"},{\"name\":\"配置3\",\"type\":\"4\",\"required\":\"false\",\"options\":\"选项1，选项2，选项3\"}]}";

        /// <summary>
        /// 变换的时候集合是空的
        /// </summary>
        [TestMethod]
        public void TransformListIsEmpty()
        {
            var code = "{\"config\":[],\"description\":\"类型描述\",\"id\":13,\"markedCode\":\"test\",\"name\":\"测试\",\"orderIndex\":1}";
            DTObject dto = DTObject.CreateReusable(code);
            dto.Transform("id=>typeId;config=>attached");
            dto.Transform("attached.options=options", (v) =>
            {
                var options = ((object[])v).Select((t) => { return (string)t; });
                return string.Join(",", options);
            });
            Assert.AreEqual("{\"attached\":[],\"description\":\"类型描述\",\"markedCode\":\"test\",\"name\":\"测试\",\"orderIndex\":1,\"typeId\":13}", dto.GetCode(true));
        }


        private const string _code1 = "{\"config\":[{\"message\":\"\",\"name\":\"1\",\"options\":[\"选项1\",\"选项2\"],\"required\":true,\"type\":4,\"persons\":[{id:\"1\",name:\"姓名1\"},{id:\"2\",name:\"姓名2\"}]}],\"description\":\"111\",\"id\":7,\"markedCode\":\"1\",\"name\":\"123\",\"orderIndex\":1,\"rootId\":6}";

        /// <summary>
        /// 保留语句
        /// </summary>
        [TestMethod]
        public void Reserve()
        {
            DTObject dto = DTObject.CreateReusable(_code1);
            dto.Transform("~config.name,config.options,config.persons,description,id");
            Assert.AreEqual("{\"config\":[{\"name\":\"1\",\"options\":[\"选项1\",\"选项2\"],\"persons\":[{\"id\":\"1\",\"name\":\"姓名1\"},{\"id\":\"2\",\"name\":\"姓名2\"}]}],\"description\":\"111\",\"id\":7}", dto.GetCode());

            dto = DTObject.CreateReusable(_code1);
            dto.Transform("~config.name,config.options,config.persons.id,description,id");
            Assert.AreEqual("{\"config\":[{\"name\":\"1\",\"options\":[\"选项1\",\"选项2\"],\"persons\":[{\"id\":\"1\"},{\"id\":\"2\"}]}],\"description\":\"111\",\"id\":7}", dto.GetCode());
        }

        /// <summary>
        /// 移除语句
        /// </summary>
        [TestMethod]
        public void Remove()
        {
            DTObject dto = DTObject.CreateReusable(_code1);
            dto.Transform("!config.name,config.options,config.persons,description,id");
            Assert.AreEqual("{\"config\":[{\"message\":\"\",\"required\":true,\"type\":4}],\"markedCode\":\"1\",\"name\":\"123\",\"orderIndex\":1,\"rootId\":6}", dto.GetCode(true));

            dto = DTObject.CreateReusable(_code1);
            dto.Transform("!config.name,config.options,config.persons.id,description,id");
            Assert.AreEqual("{\"config\":[{\"message\":\"\",\"persons\":[{\"name\":\"姓名1\"},{\"name\":\"姓名2\"}],\"required\":true,\"type\":4}],\"markedCode\":\"1\",\"name\":\"123\",\"orderIndex\":1,\"rootId\":6}", dto.GetCode(true));
        }

        /// <summary>
        /// 设置自己
        /// </summary>
        [TestMethod]
        public void SetSelf()
        {
            var dto = DTObject.CreateReusable();
            dto.SetValue(2);
            Assert.AreEqual(2, dto.GetValue<int>());

            var newDTO = DTObject.CreateReusable("{id:3}");
            dto.SetValue(newDTO); //该表达式表示设置自己
            Assert.AreEqual("{\"id\":3}", dto.GetCode());

        }

        /// <summary>
        /// 映射的对象内含有DTO成员
        /// </summary>
        [TestMethod]
        public void MapInnerDTO()
        {
            var code = "{\"name\":\"类型名称\",\"orderIndex\":\"1\",\"markedCode\":\"markedCode\",\"description\":\"描述\",\"coverConfig\":[{\"name\":\"1\",\"width\":\"111\",\"height\":\"111\"}],\"dcSlimConfig\":{\"items\":[{\"name\":\"配置1\",\"type\":\"2\",\"required\":\"false\",\"options\":[\"选项1\",\"选项2\",\"选项3\"]},{\"name\":\"配置2\",\"type\":\"4\",\"required\":\"true\",\"options\":[\"选项1\",\"选项2\"]}]}}";
            var para = DTObject.CreateReusable(code);

            var temp = DTObject.Deserialize<MapInnerDTOClass>(para);
            Assert.AreEqual("\"dcSlimConfig\":{\"items\":[{\"name\":\"配置1\",\"options\":[\"选项1\",\"选项2\",\"选项3\"],\"required\":\"false\",\"type\":\"2\"},{\"name\":\"配置2\",\"options\":[\"选项1\",\"选项2\"],\"required\":\"true\",\"type\":\"4\"}]}", temp.DCSlimConfig.GetCode(true));

            var dto = DTObject.Serialize(temp, false);
            var dcSlimConfig = dto.GetObject("dcSlimConfig");
            Assert.AreEqual("\"dcSlimConfig\":{\"items\":[{\"name\":\"配置1\",\"options\":[\"选项1\",\"选项2\",\"选项3\"],\"required\":\"false\",\"type\":\"2\"},{\"name\":\"配置2\",\"options\":[\"选项1\",\"选项2\"],\"required\":\"true\",\"type\":\"4\"}]}", temp.DCSlimConfig.GetCode(true));

        }

        [DTOClass]
        public class MapInnerDTOClass
        {
            /// <summary>
            /// 类型名称（模糊查询）
            /// </summary>
            [DTOMember("name")]
            public string Name;

            /// <summary>
            /// 
            /// </summary>
            [DTOMember("orderIndex")]
            public short OrderIndex;

            /// <summary>
            /// 
            /// </summary>
            [DTOMember("markedCode")]
            public string MarkedCode;

            /// <summary>
            /// 
            /// </summary>
            [DTOMember("description")]
            public string Description;

            [DTOMember("dcSlimConfig")]
            public DTObject DCSlimConfig;

        }


        [TestMethod]
        public void DynamicDTO()
        {
            var dto = DTObject.CreateReusable("{name:\"张三丰\",sex:'男',person:{name:'张无忌',sex:'男'},persons:[{id:1,name:'1的名称'},{id:2,name:'2的名称'}], values:[1,2,3]}");
            dynamic d = (dynamic)dto;

            var name = d.Name;
            Assert.AreEqual("张三丰", name);

            var sex = d.GetValue<string>("sex");
            Assert.AreEqual("男", sex);

            var height = d.Height;
            Assert.IsNull(height);


            name = d.person.name;
            Assert.AreEqual("张无忌", name);


            Person person = d.person;
            Assert.AreEqual("张无忌", person.Name);

            var persons = d.persons;
            Assert.AreEqual("1的名称", persons[0].Name);
            Assert.AreEqual("2的名称", persons[1].Name);

            var values = d.values.OfType<int>();
            Assert.AreEqual(1, values[0]);

        }

        private class Person
        {
            public string Name { get; set; }

            public string Sex { get; set; }
        }




        [TestMethod]
        public void MenuMapDTO()
        {
            var menu = CreateMenu();

            var dto = DTObject.CreateReusable("{name,index,parent:{name}}", menu);
            var code = dto.GetCode(true);
            Assert.AreEqual("{\"Index\":1,\"Name\":\"主菜单\",\"Parent\":{\"Name\":\"根菜单\"}}", code);

            dto = DTObject.CreateReusable("{name,index,parent,owner}", menu);
            code = dto.GetCode(true);
            Assert.AreEqual("{\"Index\":1,\"Name\":\"主菜单\",\"Parent\":{\"Index\":0,\"Name\":\"根菜单\",\"Owner\":{\"Creator\":{\"Name\":\"创建人\",\"Sex\":\"男\"},\"Id\":\"project1\",\"Name\":\"项目1\"}}}", code);

        }

        private Menu CreateMenu()
        {
            var root = new Menu();
            root.Name = "根菜单";
            root.Index = 0;
            root.Childs = new List<Menu>();
            root.Owner = new Project() { Name = "项目1", Id = "project1" };
            root.Owner.Creator = new Person() { Name = "创建人", Sex = "男" };

            var menu = new Menu();
            menu.Name = "主菜单";
            menu.Index = 1;


            root.Childs.Add(menu);
            menu.Parent = root;

            menu.Childs = new List<Menu>();
            menu.Childs.Add(new Menu() { Name = "子菜单1", Index = 2 });

            menu.Childs.Add(new Menu() { Name = "子菜单2", Index = 3 });

            var childMenu3 = new Menu() { Name = "子菜单3", Index = 4 };
            childMenu3.Childs = new List<Menu>();
            childMenu3.Childs.Add(new Menu() { Name = "子菜单3-1", Index = 5 });

            childMenu3.Childs.Add(new Menu() { Name = "子菜单3-2", Index = 6 });
            menu.Childs.Add(childMenu3);

            return menu;
        }

        [TestMethod]
        public void MenuListMapDTO()
        {
            var menu = CreateMenu();

            var dto = DTObject.CreateReusable("{name,childs:[{name,index}]}", menu);
            var code = dto.GetCode(true);
            Assert.AreEqual("{\"Childs\":[{\"Index\":2,\"Name\":\"子菜单1\"},{\"Index\":3,\"Name\":\"子菜单2\"},{\"Index\":4,\"Name\":\"子菜单3\"}],\"Name\":\"主菜单\"}", code);

            dto = DTObject.CreateReusable("{name,childs}", menu);
            code = dto.GetCode(true);
            Assert.AreEqual("{\"Childs\":[{\"Childs\":[],\"Name\":\"子菜单1\"},{\"Childs\":[],\"Name\":\"子菜单2\"},{\"Childs\":[{\"Childs\":[],\"Name\":\"子菜单3-1\"},{\"Childs\":[],\"Name\":\"子菜单3-2\"}],\"Name\":\"子菜单3\"}],\"Name\":\"主菜单\"}", code);
        }

        [TestMethod]
        public void MenuListMapObject()
        {
            var code = "{\"Childs\":[{\"Childs\":[],\"Name\":\"子菜单1\"},{\"Childs\":[],\"Name\":\"子菜单2\"},{\"Childs\":[{\"Childs\":[],\"Name\":\"子菜单3-1\"},{\"Childs\":[],\"Name\":\"子菜单3-2\"}],\"Name\":\"子菜单3\"}],\"Name\":\"主菜单\"}";
            var dto = DTObject.CreateReusable(code);
            var menu = dto.Save<Menu>();
            Assert.AreEqual(3, menu.Childs.Count);
        }

        private class Menu
        {
            public Menu Parent { get; set; }

            public string Name { get; set; }

            public int Index { get; set; }

            public List<Menu> Childs { get; set; }

            public Project Owner { get; set; }

        }

        private class Project
        {
            public string Name { get; set; }

            public string Id { get; set; }

            public Person Creator { get; set; }

        }

        [TestMethod]
        public void DTOEquals()
        {
            var code1 = "{\"Name\":\"主菜单\",value:1}";
            var code2 = "{value:1,\"name\":\"主菜单\"}";
            var dto1 = DTObject.CreateReusable(code1);
            var dto2 = DTObject.CreateReusable(code2);
            Assert.AreEqual(dto1, dto2);
        }

        [TestMethod]
        public void Multithreading()
        {
            //to do...
        }

    }
}
