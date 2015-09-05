#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace DeveloperSharp
{
    internal class Program
    {
        private static List<GameObject> _lstGameObjects = new List<GameObject>();
        private static List<GameObject> _lstObjCloseToMouse = new List<GameObject>();
        private static Menu _config;
        private static int _lastUpdateTick;
        private static int _lastMovementTick;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += sD =>
            {
                InitMenu();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
            };
        }

        private static void InitMenu()
        {
            _config = new Menu("Developer#", "developersharp", true);
            _config.AddItem(new MenuItem("range", "Max object dist from cursor").SetValue(new Slider(400, 100, 1000)));
            _config.AddToMainMenu();
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - _lastUpdateTick > 150)
            {
                _lstGameObjects = ObjectManager.Get<GameObject>().ToList();
                _lstObjCloseToMouse =
                    _lstGameObjects.Where(
                        o =>
                            o.Position.Distance(Game.CursorPos) < _config.Item("range").GetValue<Slider>().Value &&
                            !(o is Obj_Turret) && o.Name != "missile" && !(o is Obj_LampBulb) &&
                            !(o is Obj_SpellMissile) && !(o is MissileClient) && !(o is GrassObject) && !(o is DrawFX) &&
                            !(o is LevelPropSpawnerPoint) && !(o is Obj_GeneralParticleEmitter) &&
                            !o.Name.Contains("MoveTo")).ToList();
                _lastUpdateTick = Environment.TickCount;
            }
            if (Environment.TickCount - _lastMovementTick > 140000)
            {
                ObjectManager.Player.IssueOrder(
                    GameObjectOrder.MoveTo, ObjectManager.Player.Position.Randomize(-1000, 1000));
                _lastMovementTick = Environment.TickCount;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            foreach (var obj in _lstObjCloseToMouse)
            {
                if (!obj.IsValid)
                {
                    return;
                }

                var x = Drawing.WorldToScreen(obj.Position).X;
                var y = Drawing.WorldToScreen(obj.Position).Y;

                Drawing.DrawText(x, y, Color.DarkTurquoise, "IsAttackable: " + (obj is AttackableUnit));
                Drawing.DrawText(x, y + 10, Color.DarkTurquoise, "IsBase: " + (obj is Obj_AI_Base));
                Drawing.DrawText(x, y + 20, Color.DarkTurquoise, "IsMinion: " + (obj is Obj_AI_Minion));
                Drawing.DrawText(x, y + 30, Color.DarkTurquoise, "IsHero: " + (obj is Obj_AI_Hero));

                if (obj is Obj_AI_Base)
                {
                    var aiobj = obj as Obj_AI_Base;
                    Drawing.DrawText(x, y + 40, Color.DarkTurquoise, "BaseName: " + aiobj.CharData.BaseSkinName);
                    Drawing.DrawText(x, y + 50, Color.DarkTurquoise, "Name: " + aiobj.CharData.Name);
                }
                else if (obj is AttackableUnit)
                {
                    var unit = obj as AttackableUnit;
                    Drawing.DrawText(x, y + 40, Color.DarkTurquoise, "Name: " + unit.Name);
                }
            }
        }
    }
}
