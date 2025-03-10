using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace HikanyanLaboratory.Script
{
    public class Matching : MonoBehaviour
    {
        public List<StatusData> TeamA = new List<StatusData>();
        public List<StatusData> TeamB = new List<StatusData>();
        public List<StatusData> AllCharacters = new List<StatusData>();
        private List<string> _logList = new List<string>();

        private void Start()
        {
            Initialize();
            SimulateBattle();
        }

        public void Initialize()
        {
            for (int i = 0; i < 5; i++)
            {
                TeamA.Add(new StatusData
                {
                    _name = $"TeamA_{i + 1}",
                    _hp = 100,
                    _previousHp = 100,
                    _atk = UnityEngine.Random.Range(10, 20),
                    _def = UnityEngine.Random.Range(5, 15),
                    _spd = UnityEngine.Random.Range(10, 30)
                });

                TeamB.Add(new StatusData
                {
                    _name = $"TeamB_{i + 1}",
                    _hp = 100,
                    _previousHp = 100,
                    _atk = UnityEngine.Random.Range(10, 20),
                    _def = UnityEngine.Random.Range(5, 15),
                    _spd = UnityEngine.Random.Range(10, 30)
                });
            }

            AllCharacters.AddRange(TeamA);
            AllCharacters.AddRange(TeamB);
            SortBySpeed();
        }

        public void SimulateBattle()
        {
            int turn = 1;
            for (; turn <= 5; turn++)
            {
                CachePreviousState(); // 現在の状態をキャッシュ
                
                var pairs = MatchPairs();
                foreach (var pair in pairs)
                {
                    ResolveAttack(pair.attacker, pair.defender);
                }

                SortBySpeed();
                DisplayLog(turn); // ターン終了ログ
                string log = string.Join("\n", _logList);
                Debug.Log(log);
                _logList.Clear();
            }
        }

        void CachePreviousState()
        {
            foreach (var character in AllCharacters)
            {
                character._previousHp = character._hp;
            }
        }

        void DisplayLog(int turn)
        {
            _logList.Add(new string('+', 40));
            _logList.Add($"<Color=green>{"Turn",-10} {turn}</Color>");
            _logList.Add(new string('-', 40));

            foreach (var character in AllCharacters)
            {
                string hpChange = character._previousHp != character._hp
                    ? $"<color=red>{character._previousHp,3} → {character._hp,3}</color>"
                    : $"{character._previousHp,3} → {character._hp,3}";

                string atkChange = $"{character._atk,3} → {character._atk,3}";
                string defChange = $"{character._def,3} → {character._def,3}";
                string spdChange = $"{character._spd,3} → {character._spd,3}";

                string status = string.Format(
                    "{0,-10} HP = {1}   ATK = {2}   DEF = {3}   SPD = {4}",
                    character._name,
                    hpChange,
                    atkChange,
                    defChange,
                    spdChange
                );

                _logList.Add(status);
            }
        }

        public List<(StatusData attacker, StatusData defender)> MatchPairs()
        {
            var pairs = new List<(StatusData attacker, StatusData defender)>();
            var matched = new HashSet<StatusData>();

            foreach (var character in AllCharacters)
            {
                if (matched.Contains(character)) continue;

                StatusData target = null;
                if (TeamA.Contains(character))
                {
                    target = TeamB
                        .Where(t => !matched.Contains(t))
                        .OrderByDescending(t => EvaluateMatchPriority(character, t))
                        .FirstOrDefault();
                }
                else if (TeamB.Contains(character))
                {
                    target = TeamA
                        .Where(t => !matched.Contains(t))
                        .OrderByDescending(t => EvaluateMatchPriority(character, t))
                        .FirstOrDefault();
                }

                if (target != null)
                {
                    pairs.Add((character, target));
                    matched.Add(character);
                    matched.Add(target);
                }
            }

            return pairs;
        }

        public void ResolveAttack(StatusData attacker, StatusData defender)
        {
            float baseDamage = Mathf.Max(1, attacker._atk - defender._def);
            defender._hp -= (int)baseDamage;
            defender._hp = Mathf.Max(0, defender._hp); // HPは0未満にならない
            Debug.Log(
                $"<color=cyan>{attacker._name}</color> attacks <color=yellow>{defender._name}</color> for <color=red>{(int)baseDamage}</color> damage. <color=yellow>{defender._name}</color> has <color=green>{defender._hp}</color> HP remaining.");
        }
        
        private int EvaluateMatchPriority(StatusData attacker, StatusData defender)
        {
            // 評価基準: 攻撃力、速度、残りHPの重み付き評価
            int atkWeight = 3;
            int spdWeight = 2;
            int hpWeight = 1;

            return (defender._atk * atkWeight) + (defender._spd * spdWeight) - (defender._hp * hpWeight);
        }


        public void SortBySpeed()
        {
            AllCharacters.Sort((a, b) => b._spd - a._spd);
        }
    }

    public class StatusData
    {
        public string _name;
        public int _hp;
        public int _previousHp;
        public int _atk;
        public int _def;
        public int _spd;
        
        
    }
}