public class SubDungeon
    {
        public SubDungeon left, right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0); // i.e null
        public int debugId;
        public List<Rect> corridors = new List<Rect>();
        public Rect boundaryLine;

        private static int debugCounter = 0;

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
            bool splitH = false;
            bool splitV = false;
            float minSplitRate = 0.25f;

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

            if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            {
                //Debug.Log("Sub-dungeon " + debugId + " will be a leaf");
                return false;
            }

            // 가로로 자르려는 경우
            if (splitH)
            {
                // 잘랐을때 최소크기 이하면 자르지 않습니다.
                if (rect.height <= minRoomSize * 2)
                    return false;

                // 방은 최소크기 혹은 최소비율 이상으로 잘려야 합니다.
                minRoomSize = (int)Mathf.Max(minRoomSize, rect.height * minSplitRate);

                // 양쪽 구획이 최소크기 이상이 되도록 보정합니다.
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                // 가로로 잘려진 두개의 구획 생성
                left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new SubDungeon(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
                left.boundaryLine = new Rect(rect.x, rect.y + split, rect.width, 1);
                right.boundaryLine = new Rect(rect.x, rect.y + split, rect.width, 1);
            }
            // 세로로 자르려는 경우
            else if (splitV)
            {
                // 잘랐을때 최소크기 이하면 자르지 않습니다.
                if (rect.width <= minRoomSize * 2)
                    return false;

                // 방은 최소크기 혹은 최소비율 이상으로 잘려야 합니다.
                minRoomSize = (int)Mathf.Max(minRoomSize, rect.width * minSplitRate);

                // 양쪽 구획이 최소크기 이상이 되도록 보정합니다.
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                // 세로로 잘려진 두개의 구획 생성
                left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new SubDungeon(new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
                left.boundaryLine = new Rect(rect.x + split, rect.y, 1, rect.height);
                right.boundaryLine = new Rect(rect.x + split, rect.y, 1, rect.height);
            }
            // 그 외의 경우가 있을 수 있을까요?
            else
            {
                Debug.LogError("Room spliting error");
            }

            return true;
        }

        public void CreateRoom()
        {
            if (left != null)
            {
                left.CreateRoom();
            }
            if (right != null)
            {
                right.CreateRoom();
            }
            if (left != null && right != null)
            {
                CreateCorridorBetween(left, right);
            }
            if (IAmLeaf())
            {
                int roomWidth = (int)Random.Range(rect.width * 0.75f, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height * 0.75f, rect.height - 2);
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                // room position will be absolute in the board, not relative to the sub-dungeon
                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                //Debug.Log("Created room " + room + " in sub-dungeon " + debugId + " " + rect);
            }
        }


        public void CreateCorridorBetween(SubDungeon left, SubDungeon right)
        {
            Rect lroom = left.GetRoom();
            Rect rroom = right.GetRoom();
            //todo: make corridorWidth changeable at unity inspector
            int corridorWidth = 1;

            //Debug.Log("Creating corridor(s) between " + left.debugId + "(" + lroom + ") and " + right.debugId + " (" + rroom + ")");

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
    }

    public void CreateBSP(SubDungeon subDungeon)
    {
        //Debug.Log("Splitting sub-dungeon " + subDungeon.debugId + ": " + subDungeon.rect);
        if (subDungeon.IAmLeaf())
        {
            // if the sub-dungeon is too large split it
            if (subDungeon.rect.width > maxRoomSize
                || subDungeon.rect.height > maxRoomSize
                || Random.Range(0.0f, 1.0f) > 0.25)
            {

                if (subDungeon.Split(minRoomSize, maxRoomSize))
                {
                    //Debug.Log("Splitted sub-dungeon " + subDungeon.debugId + " in "
                     //   + subDungeon.left.debugId + ": " + subDungeon.left.rect + ", "
                      //  + subDungeon.right.debugId + ": " + subDungeon.right.rect);

                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);
                }
            }
        }
    }