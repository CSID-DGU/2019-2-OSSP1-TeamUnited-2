﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubDungeon
    {
        // 하위 서브던전
        public SubDungeon left, right;

        // 서브던전의 실체를 나타내는 전체 구획
        public Rect rect;

        // 이하 서브던전이 room일때 사용되는 변수
        public Rect room = new Rect(-1, -1, 0, 0); // i.e null
        public List<Rect> corridors = new List<Rect>();
        public List<GameObject> monsters    = new List<GameObject>();
        public List<GameObject> items       = new List<GameObject>();

        // 이하 서브던전이 room이 아닐 때 사용되는 변수
        public Rect partition = new Rect (-1, -1, 0, 0); 
        public enum Alignment { none = 0, vertical, horizontal };
        public Alignment partitionAlignment = Alignment.none;
        public List<Rect> tunnels = new List<Rect>();
        
        // 이하 디버그 변수
        public int debugId;
        private static int debugCounter = 0;
        protected bool savedToList = false;
        protected RoomType roomType;
        public Rect boundaryLine;

        public SubDungeon(Rect mrect)
        {
            rect = mrect;
            debugId = debugCounter;
            debugCounter++;
        }

        public bool IAmLeaf()
        {
            return left == null && right == null;
        }

        public bool Split(int minRoomSize, int maxRoomSize)
        {
            // 방이 아니면 자르지 않습니다.
            if (!IAmLeaf())
            {
                return false;
            }

            // choose a vertical or horizontal split depending on the proportions
            // i.e. if too wide split vertically, or too long horizontally,
            // or if nearly square choose vertical or horizontal at random
            float minSplitRate = 0.4f;
            bool splitH = false;
            bool splitV = false;

            // 가로로 길쭉한 맵은 세로로 자릅니다.
            if (rect.width / rect.height >= 1.05)
            {
                splitV = true;
            }
            // 세로로 길쭉한 맵은 가로로 자릅니다.
            else if (rect.height / rect.width >= 1.05)
            {
                splitH = true;
            }
            // 비슷비슷하면 랜덤으로 자릅니다
            else
            {
                splitH = Random.Range(0.0f, 1.0f) > 0.5;
                splitV = true;
            }

            // if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            // {
            //     Debug.Log("Sub-dungeon " + debugId + " will be a leaf");
            //     return false;
            // }

            // 가로로 자르려는 경우
            if (splitH)
            {
                // 잘랐을때 최소크기 이하면 자르지 않습니다.
                if (rect.height <= minRoomSize * 2)
                    return false;
                    
                // 방은 최소크기 혹은 최소비율 이상으로 잘려야 합니다.
                minRoomSize = (int)Mathf.Max(minRoomSize, rect.height * minSplitRate);
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                // 가로로 잘려진 두개의 구획 생성 & 하위 노드에 편입 
                left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new SubDungeon(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));

                // 하위 노드에 바운더리 위치 할당
                left.boundaryLine = new Rect(rect.x, rect.y + split, rect.width, 1);
                right.boundaryLine = new Rect(rect.x, rect.y + split, rect.width, 1);

                // 자기 자신에게는 가로로 긴 파티션 할당
                partition = new Rect(rect.x, rect.y + split, rect.width, 0);
                partitionAlignment = Alignment.horizontal;
            }
            // 세로로 자르려는 경우
            else if (splitV)
            {
                // 잘랐을때 최소크기 이하면 자르지 않습니다.
                if (rect.width <= minRoomSize * 2)
                    return false;

                // 방은 최소크기 혹은 최소비율 이상으로 잘려야 합니다.
                minRoomSize = (int)Mathf.Max(minRoomSize, rect.width * minSplitRate);
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                // 세로로 잘려진 두개의 구획 생성 & 하위 노드에 편입
                left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new SubDungeon(new Rect(rect.x + split, rect.y, rect.width - split, rect.height));

                // 하위 노드에 바윈더리 위치 할당
                left.boundaryLine = new Rect(rect.x + split, rect.y, 1, rect.height);
                right.boundaryLine = new Rect(rect.x + split, rect.y, 1, rect.height);
                
                // 자기 자신에게는 세로로 긴 파티션 할당
                partition = new Rect(rect.x + split, rect.y, 0, rect.height);
                partitionAlignment = Alignment.vertical;
            }
            // 그 외의 경우가 있을 수 있을까요?
            else
            {
                Debug.LogError("Room spliting error");
            }
            return true;
        }

        public void CreateRoomRecursive()
        {
            if (left != null)
            {
                left.CreateRoomRecursive();
            }
            if (right != null)
            {
                right.CreateRoomRecursive();
            }
            // 서브던전이 room이 아닌 경우입니다.
            if (!IAmLeaf())
            {
                CreateTunnel();
                // CreateCorridorBetween(left, right);
            }
            // 서브던전이 room인 경우입니다.
            if (IAmLeaf())
            {
                // room의 크기를 결정해줍니다.
                int roomWidth = (int)Random.Range(rect.width * 0.75f, rect.width * 0.85f);
                int roomHeight = (int)Random.Range(rect.height * 0.75f, rect.height * 0.85f);

                // room 은 rect 내부의 들어갈 수 있는 랜덤한 위치 아무데나 들어갑니다.
                int roomX = (int)(rect.width - roomWidth) / 2;
                // (int)Random.Range(3, rect.width - roomWidth - 3);
                int roomY = (int)(rect.height - roomHeight) / 2;
                // (int)Random.Range(3, rect.height - roomHeight - 3);

                // room position will be absolute in the board, not relative to the sub-dungeon
                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
            }
        }

        public void CreateTunnel()
        {
            Rect lroom = left.GetRoom();
            Rect rroom = right.GetRoom();
            int tunnelWidth = 6;

            // 터널 깊이는 양쪽 방에서 추출된 랜덤 방의 평균 변 길이의 절반입니다.
            int tunnelDepth = (int)((lroom.width + lroom.height + rroom.width + rroom.height) / 4);
            tunnelDepth = (int)(tunnelDepth * 0.5);

            // 파티션의 모양에 따라 터널을 생성합니다.
            if(partitionAlignment == Alignment.horizontal)
            {
                float tunnelStartingPoint = Random.Range(partition.x + partition.width * 0.25f, partition.x + partition.width * 0.75f);
                Rect tunnel = new Rect(tunnelStartingPoint - (tunnelWidth / 2), partition.y - tunnelDepth, tunnelWidth, tunnelDepth * 2);
                tunnels.Add(tunnel);
            }
            else if (partitionAlignment == Alignment.vertical)
            {
                float tunnelStartingPoint = Random.Range(partition.y + partition.height * 0.25f, partition.y + partition.height * 0.75f);
                Rect tunnel = new Rect(partition.x - tunnelDepth, tunnelStartingPoint - (tunnelWidth / 2), tunnelDepth * 2, tunnelWidth);
                tunnels.Add(tunnel);
            }
            else
            {
                Debug.LogError("Try to refer uncreated partition");
            }         
        }

        public void CreateCorridorBetween(SubDungeon left, SubDungeon right)
        {
            Rect lroom = left.GetRoom();
            Rect rroom = right.GetRoom();
            //todo: make corridorWidth changeable at unity inspector
            int corridorWidth = 2;

            Debug.Log("Creating corridor(s) between " + left.debugId + "(" + lroom + ") and " + right.debugId + " (" + rroom + ")");

            // attach the corridor to a random point in each room
            Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
            Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

            // always be sure that left point is on the left to simplify the code
            if (lpoint.x > rpoint.x)
            {
                Vector2 temp = lpoint;
                lpoint = rpoint;
                rpoint = temp;
            }

            int w = (int)(lpoint.x - rpoint.x);
            int h = (int)(lpoint.y - rpoint.y);

            //Debug.Log("lpoint: " + lpoint + ", rpoint: " + rpoint + ", w: " + w + ", h: " + h);

            // if the points are not aligned horizontally
            if (w != 0)
            {
                //boundaryLine이 가로로 있는 경우
                if (left.boundaryLine.height == 1)
                {
                    //lpoint가 rpoint 보다 낮은 경우
                    if (h < 0)
                    {
                        //draw corridors from lpoint to boundaryLine
                        corridors.Add(new Rect(lpoint.x, lpoint.y, corridorWidth, Mathf.Abs((int)(lpoint.y - left.boundaryLine.y))));
                        //draw corridors fallowing boundaryLine
                        corridors.Add(new Rect(lpoint.x, left.boundaryLine.y, Mathf.Abs(w) + 1, corridorWidth));
                        //draw corridors form boundaryLine to rpoint
                        corridors.Add(new Rect(rpoint.x, left.boundaryLine.y, corridorWidth, Mathf.Abs((int)(rpoint.y - left.boundaryLine.y))));
                    }
                    //lpoint가 rpoint 보다 높은 경우
                    else
                    {
                        //draw corridors from lpoint to boundaryLine
                        corridors.Add(new Rect(lpoint.x, left.boundaryLine.y, corridorWidth, Mathf.Abs((int)(lpoint.y - left.boundaryLine.y))));
                        //draw corridors fallowing boundaryLine
                        corridors.Add(new Rect(lpoint.x, left.boundaryLine.y, Mathf.Abs(w) + 1, corridorWidth));
                        //draw corridors form boundaryLine to rpoint
                        corridors.Add(new Rect(rpoint.x, rpoint.y, corridorWidth, Mathf.Abs((int)(rpoint.y - left.boundaryLine.y))));
                    }
                }
                //bundaryLine이 세로로 있는 경우
                else if (left.boundaryLine.width == 1)
                {
                    //lpoint 가 rpoint 보다 낮은 경우
                    if (h < 0)
                    {
                        //draw corridors from lpoint to boundaryLine
                        corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs((int)(lpoint.x - left.boundaryLine.x)), corridorWidth));
                        //draw corridors fallowing boundaryLine
                        corridors.Add(new Rect(left.boundaryLine.x, lpoint.y, corridorWidth, Mathf.Abs(h)));
                        //draw corridors form boundaryLine to rpoint
                        corridors.Add(new Rect(left.boundaryLine.x, rpoint.y, Mathf.Abs((int)(left.boundaryLine.x - rpoint.x)), corridorWidth));
                    }
                    //lpoint 가 rpoint 보다 높은 경우
                    else
                    {
                        //draw corridors from lpoint to boundaryLine
                        corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs((int)(lpoint.x - left.boundaryLine.x)), corridorWidth));
                        //draw corridors fallowing boundaryLine
                        corridors.Add(new Rect(left.boundaryLine.x, rpoint.y, corridorWidth, Mathf.Abs(h) + 1));
                        //draw corridors form boundaryLine to rpoint
                        corridors.Add(new Rect(left.boundaryLine.x, rpoint.y, Mathf.Abs((int)(left.boundaryLine.x - rpoint.x)), corridorWidth));
                    }
                }
                //error
                else
                {
                    //Debug.LogError("Create corridor error");
                }
            }
            else
            {
                // if the points are aligned horizontally
                // go up or down depending on the positions
                if (h < 0)
                {
                    corridors.Add(new Rect((int)lpoint.x, (int)lpoint.y, corridorWidth, Mathf.Abs(h)));
                }
                else
                {
                    corridors.Add(new Rect((int)rpoint.x, (int)rpoint.y, corridorWidth, Mathf.Abs(h)));
                }
            }

            //Debug.Log("Corridors: ");
            foreach (Rect corridor in corridors)
            {
                //Debug.Log("corridor: " + corridor);
            }
        }

        public Rect GetRoom()
        {
            if (IAmLeaf())
            {
                return room;
            }
            if (left != null)
            {
                Rect lroom = left.GetRoom();
                if (lroom.x != -1)
                {
                    return lroom;
                }
            }
            if (right != null)
            {
                Rect rroom = right.GetRoom();
                if (rroom.x != -1)
                {
                    return rroom;
                }
            }

            // workaround non nullable structs
            return new Rect(-1, -1, 0, 0);
        }
        public SubDungeon RandomPopDungeon()
        // 던전이 빌때까지 차례로 방을 반환합니다.
        {
            // 자신이 말단 서브던전이고, 아직 pop된적이 없으면 반환합니다.
            if (IAmLeaf() && !savedToList)
            {
                savedToList = true;
                return this;
            }
            // 자신이 말단 서브던전이지만, 이미 pop되었다면 null을 반환합니다.
            else if (IAmLeaf() && savedToList)
            {
                return null;
            }
            else
            {
                // 자신이 말단이 아닌경우, left부터 차례로 연쇄호출을 시도합니다.
                // left 연쇄호출에서 말단 노드를 발견했다면 반환합니다.
                SubDungeon nextPop = left.RandomPopDungeon();
                if(nextPop != null)
                {
                    return nextPop;
                }

                // left 연쇄호출에서 말단 노드를 찾지 못했을 경우, right 연쇄호출에서 말단 노드를 발견했다면 반환합니다.
                nextPop = right.RandomPopDungeon();
                if (nextPop != null)
                {
                    return nextPop;
                }
                
                // 더이상 반환할 것이 없습니다. null를 반환합니다.
                return null;
            }
        }   
    }

    