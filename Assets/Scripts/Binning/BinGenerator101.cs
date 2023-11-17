using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;
using System.Globalization;
using Unity.Jobs;
using System.Runtime.InteropServices;


public class BinGenerator101 : MonoBehaviour
{  
   public Camera mainCamera;
   private List<GameObject> AllObjects = new List<GameObject>();
   private List<int> ObjOffset = new List<int>();
   private Dictionary<string, int> ObjToOffset = new Dictionary<string, int>();
   private Dictionary<string, float> ObjLength = new Dictionary<string,float>();
   private int Offset;
   private float binSize;
   private int binNum; 
   private Dictionary<string,int> numBinsHeight = new Dictionary<string,int>();
   private Dictionary<string,Vector3> BottomLeftPos = new Dictionary<string, Vector3>();
   private Dictionary<string, int> numBinsWidth = new Dictionary<string, int>();
   private List <string[]> ObjInScene = new List<string[]>();
   public int numOfBinsForFloorLength; //Default 40 
   public float FloorLength; // 25 unity units
   public string OutputFile;
   public string InputFile;

   private string[] walltag;
   private string[] greenpillartag;
   private string[] bluepillartag; 
   private string[] redpillartag; 
   private string[] yellowpillartag; 
   private string[] catPlaneVertical; 
   private string[] catPlaneHorizontal; 
   private string[] catbase;
   private string[] pigPlaneHorizontal; 
   private string[] pigPlaneVertical; 
   private string[] pigbase;
   private string[] crocodilePlaneHorizontal; 
   private string[] crocodilePlaneVertical; 
   private string[] crocodilebase;
   private string[] hippoPlaneHorizontal; 
   private string[] hippoPlaneVertical; 
   private string[] hippobase; 
   private string[] rabbitPlaneHorizontal;
   private string[] rabbitPlaneVertical; 
   private string[] rabbitbase;
   private string[] foxPlaneHorizontal; 
   private string[] foxPlaneVertical; 
   private string[] foxbase; 

   //stores all data excluding "Sampled Ignored" data 
   private List<string[]> csvData = new List<string[]>();
   private List<string> Objects = new List<string>();
   private List<Vector3> Gazecoordinates = new List<Vector3>();
   private List<int> Timestamp = new List<int>();
   
   //stores data of objects within the radius density after raycasting 
   private List<int> AllTimestamps = new List<int>();
   private List<string> AllObjecthit = new List<string>();
   private List<Vector3> AllObjecthitpos = new List<Vector3>();

   //stores data of filtered bin numbers 
   private List<int> Binnumbers = new List<int>();
   private List<int> Timestamps = new List<int>();
   private List<string> Objecthit = new List<string>();
   private List<Vector3> Objecthitpos = new List<Vector3>();

   
    // Start is called before the first frame update
    void Start()
    {

    // Tags to search for
    //tags are called in order that they are binned from west clockwise
    string[] Ground = {"Ground"};
    string[] Ceiling = {"Ceiling"};
    walltag = new string[] {"wall_01", "wall_02", "wall_03", "wall_04", "wall_05", "wall_06"}; 
    greenpillartag = new string[] {"m_wall_5", "m_wall_26", "m_wall_25", "m_wall_1"}; 
    bluepillartag = new string[] {"m_wall_21", "m_wall_29", "m_wall_6", "m_wall_10"};
    redpillartag = new string[] {"m_wall_3", "m_wall_15", "m_wall_24", "m_wall_4"};
    yellowpillartag = new string [] {"m_wall_20", "m_wall_12", "m_wall_8", "m_wall_7"};
    catPlaneHorizontal = new string[] {"Plane_1", "Plane_2"};
    catPlaneVertical = new string[] { "Plane_3", "Plane_4", "Plane_5", "Plane_6"};
    catbase = new string[] {"Base_1", "Base_2", "Base_3", "Base_4"};
    pigPlaneHorizontal = new string[] { "Plane_7", "Plane_8"};
    pigPlaneVertical = new string[] {"Plane_9", "Plane_10", "Plane_11", "Plane_12"};
    pigbase = new string[] {"Base_5" , "Base_6", "Base_7", "Base_8"}; 
    crocodilePlaneHorizontal = new string[] {"Plane_13", "Plane_14"};
    crocodilePlaneVertical = new string[] { "Plane_15", "Plane_16", "Plane_17", "Plane_18"};
    crocodilebase = new string[] {"Base_9", "Base_10", "Base_11", "Base_12"};
    hippoPlaneHorizontal = new string[] {"Plane_19", "Plane_20"};
    hippoPlaneVertical = new string[] {"Plane_21", "Plane_22", "Plane_23", "Plane_24"};
    hippobase = new string[] {"Base_13", "Base_14", "Base_15", "Base_16"};
    rabbitPlaneHorizontal =new string[] {"Plane_25", "Plane_26"};
    rabbitPlaneVertical = new string[] {"Plane_27", "Plane_28", "Plane_29", "Plane_30"};
    rabbitbase = new string[] {"Base_17", "Base_18", "Base_19", "Base_20"};
    foxPlaneHorizontal = new string[] {"Plane_31", "Plane_32"};
    foxPlaneVertical = new string[] {"Plane_33", "Plane_34", "Plane_35", "Plane_36"};
    foxbase = new string[] {"Base_21", "Base_22", "Base_23", "Base_24"};
    
    ObjInScene.Add(Ground);
    ObjInScene.Add(Ceiling);
    ObjInScene.Add(walltag);
    ObjInScene.Add(greenpillartag);
    ObjInScene.Add(bluepillartag);
    ObjInScene.Add(redpillartag);
    ObjInScene.Add(yellowpillartag);
    ObjInScene.Add(catPlaneHorizontal);
    ObjInScene.Add(catPlaneVertical);
    ObjInScene.Add(catbase);
    ObjInScene.Add(pigPlaneHorizontal);
    ObjInScene.Add(pigPlaneVertical);
    ObjInScene.Add(pigbase);
    ObjInScene.Add(crocodilePlaneHorizontal);
    ObjInScene.Add(crocodilePlaneVertical);
    ObjInScene.Add(crocodilebase);
    ObjInScene.Add(hippoPlaneHorizontal);
    ObjInScene.Add(hippoPlaneVertical);
    ObjInScene.Add(hippobase);
    ObjInScene.Add(rabbitPlaneHorizontal);
    ObjInScene.Add(rabbitPlaneVertical);
    ObjInScene.Add(rabbitbase);
    ObjInScene.Add(foxPlaneHorizontal);
    ObjInScene.Add(foxPlaneVertical);
    ObjInScene.Add(foxbase);

    foreach(string[] obj in ObjInScene){

        foreach(string tagobj in obj){

        GameObject Object = GameObject.FindWithTag(tagobj);

        if(Object != null){
            AllObjects.Add(Object);
        }
      }
    }

    binSize = FloorLength/numOfBinsForFloorLength;

    ObjOffset.Add(2); //Cue and Hint Image 
    Offset = 2;

   
    foreach(GameObject obj in AllObjects){
        
        Renderer renderer = obj.GetComponent<Renderer>();
        Collider collider = obj.GetComponent<Collider>();

        if(renderer != null && collider != null){

        //1. Get the Object Position (in 3D world coordinate) and Size 

        Bounds bounds = obj.GetComponent<Renderer>().bounds; 
        Vector3 size = bounds.size;
        Vector3 objectPosition = obj.transform.position;
        Quaternion objectRotation = obj.transform.rotation;

        //2. Gets the Number of Bins required to fill the surface of each object to determine the offset 
        //Note: this code only works on surfaces of gameobjects (i.e., 2D). Objects cannot have all x,y,z dimensions (have to adjust accordingly)
        
        // Uses vector to get the axes and the correct reference position of the object. 
        // Note (Double Tee Maze): For the maze walls, the blue axes (of the object) should be facing the west and the green axes should be upwards 
        float StepX = Mathf.Sign(obj.transform.forward.x);
        float StepY = Mathf.Sign(obj.transform.forward.y);
        float StepZ = Mathf.Sign(obj.transform.forward.z); 

        float BottomLeftPosX = Mathf.Round((objectPosition.x -  StepX * (size.x/2))*1000f)/1000f;
        float BottomLeftPosY = Mathf.Round((objectPosition.y -  StepY * (size.y/2))*1000f)/1000f;
        float BottomLeftPosZ = Mathf.Round((objectPosition.z -  StepZ * (size.z/2))*1000f)/1000f; 

        BottomLeftPos.Add(obj.name, new Vector3(BottomLeftPosX, BottomLeftPosY, BottomLeftPosZ));

        //3. Finds the number of bins in the x,y,z direction 
        int numBinsX = Mathf.CeilToInt((Mathf.Round(size.x*1000f)/1000f)/ binSize); 
        int numBinsY = Mathf.CeilToInt((Mathf.Round(size.y*1000f)/1000f)/ binSize);
        int numBinsZ = Mathf.CeilToInt((Mathf.Round(size.z*1000f)/1000f)/ binSize);

    
        if(Mathf.Approximately(numBinsX, 0)){  //ZPos (North) & ZNeg(South)
            Offset += numBinsY * numBinsZ;
            numBinsWidth.Add(obj.name, numBinsZ);
            numBinsHeight.Add(obj.name, numBinsY);
            ObjLength.Add(obj.name,Mathf.Round(size.z));

        } 
        else if(Mathf.Approximately(numBinsY,0)){ //Ground and Ceiling
            Offset += numBinsX * numBinsZ;
            numBinsWidth.Add(obj.name, numBinsX);
            ObjLength.Add(obj.name, Mathf.Round(size.x));
        }
        else if(Mathf.Approximately(numBinsZ,0)){ // XPos(East) and XNeg (West)
            Offset += numBinsX * numBinsY;
            numBinsWidth.Add(obj.name, numBinsX);
            numBinsHeight.Add(obj.name, numBinsY);
            ObjLength.Add(obj.name, Mathf.Round(size.x));
        }

        ObjOffset.Add(Offset);
        //Debug.Log(obj.name + " " + Offset);
    
       }

        ObjToOffset.Add(obj.name, ObjOffset[ObjOffset.Count -2]); // stores object name: offset value in Dictionary   

    }

     ReadCSVFile(InputFile);

    foreach(KeyValuePair <string, int> kvp in ObjToOffset){
        Debug.Log("Key: "+ kvp.Key + " "+ "Value: " + kvp.Value);
    }


    /*foreach(KeyValuePair <string, Vector3> kvp in BottomLeftPos){
        Debug.Log("Key: "+ kvp.Key + " "+ "Value: " + kvp.Value);
    }*/


    /*foreach(KeyValuePair <string, float> kvp in ObjLength){
        Debug.Log("Key: "+ kvp.Key + " "+ "Value: " + kvp.Value);
    }*/

    /*foreach(KeyValuePair <string, int> kvp in numBinsHeight){

        Debug.Log("Key: " + kvp.Key + " " + " Value: " + kvp.Value);
    }*/

    foreach(KeyValuePair <string, int> kvp in numBinsWidth){

        Debug.Log("Key: " + kvp.Key + " " + " Value: " + kvp.Value);
    }


    }



    private void ReadCSVFile(string file)
    {

           if (!File.Exists(file)){
            Debug.LogError("CSV file not found!");
            return;
           }

        using (StreamReader reader = new StreamReader(file)){
            string line;
            int currentBatchSize =0; 


            while ((line = reader.ReadLine()) != null)
            { 
                if(!line.Contains("Sample Ignored")){ //filter data containing "Sampled Ignored"
                    string[] rowData = line.Split(',');
                    csvData.Add(rowData); 
                
                }
            }

            foreach(string[] values in csvData){

            int time = int.Parse(values[1].Trim());
            Timestamp.Add(time);

            string objectname = values[2].Trim();
            Objects.Add(objectname);

            CultureInfo culture = CultureInfo.InvariantCulture;

            float.TryParse(values[9], NumberStyles.Float, culture, out float xcoordinate);
            //Debug.Log("x: " + xcoordinate);

            float.TryParse(values[10], NumberStyles.Float, culture, out float ycoordinate);
            //Debug.Log("y: " + ycoordinate);
                
            float.TryParse(values[11], NumberStyles.Float, culture, out float zcoordinate);
            //Debug.Log("z: " + zcoordinate);

                
            Vector3 coordinate = new Vector3(xcoordinate, ycoordinate, zcoordinate);
            Gazecoordinates.Add(coordinate);

            }

        } 

        BinComputation(Objects, Gazecoordinates, Timestamp);
       

        /*for(int i =0; i < Timestamp.Count; i++){

            Debug.Log(Timestamp[i] + " " +  Objects[i] + " " + Gazecoordinates[i]);
        } */

    }


    /*private void ProcessChunk(List<string[]> chunk){

        foreach(string[] values in csvData){

            int time = int.Parse(values[1].Trim());
            Timestamp.Add(time);

            string objectname = values[2].Trim();
            Objects.Add(objectname);

            CultureInfo culture = CultureInfo.InvariantCulture;

            float.TryParse(values[5], NumberStyles.Float, culture, out float xcamposition);

            float.TryParse(values[6], NumberStyles.Float, culture, out float ycamposition);

            float.TryParse(values[7], NumberStyles.Float, culture, out float zcamposition);
                        
            float.TryParse(values[8], NumberStyles.Float, culture, out float ycamrotation);
            camRotation.Add(ycamrotation);

            float.TryParse(values[9], NumberStyles.Float, culture, out float xcoordinate);
            //Debug.Log("x: " + xcoordinate);

            float.TryParse(values[10], NumberStyles.Float, culture, out float ycoordinate);
            //Debug.Log("y: " + ycoordinate);
                
            float.TryParse(values[11], NumberStyles.Float, culture, out float zcoordinate);
            //Debug.Log("z: " + zcoordinate);

            Vector3 Position = new Vector3(xcamposition, ycamposition + 1.35f, zcamposition);
            camPosition.Add(Position);

                
            Vector3 coordinate = new Vector3(xcoordinate, ycoordinate, zcoordinate);
            Gazecoordinates.Add(coordinate);
                    
            
            
        }

        FindObjectsInGazeRadius(Timestamp, Objects, camPosition, camRotation, Gazecoordinates);

        //BinComputation(AllObjecttype.ToArray(), Allgazecoordinates.ToArray(), Alltimestamps.ToArray());
        /*for(int i =0; i < Timestamps.Count; i++){

            Debug.Log(Timestamps[i] + " " +  Objects[i] + " " + Gazecoordinates[i] + " " + camPosition[i] + " " + camRotation[i]);
        }
    }*/

    /*private void FindObjectsInGazeRadius(List<int> Timestamps, List<string> Objectname, List<Vector3> camPosition, List<float> camRotation ,List<Vector3> Gazecoord){


        for(int i = 0; i < Timestamps.Count; i++){

            int Raycounter = PixelsPerRay;

            if(Objectname[i] == "CueImage" || Objectname[i] == "HintImage"){

                AllTimestamps.Add(Timestamps[i]);
                AllObjecthit.Add(Objectname[i]);
                AllObjecthitpos.Add(Gazecoord[i]);


            }else{
                
                StartCoroutine(MoveCameraThroughPositions(camPosition[i], camRotation[i]));
                Vector3 viewportPoint = mainCamera.WorldToViewportPoint(Gazecoord[i]);
                

                Vector2 pixelCoords = new Vector2(viewportPoint.x * Screen.width, viewportPoint.y * Screen.height);

                for(float x = pixelCoords.x - radius; x< pixelCoords.x + radius; x++){
                    
                    for(float y = pixelCoords.y - radius; y< pixelCoords.y + radius; y++){

                    Vector2 newCoord = new Vector2 (x, y);

                    //if(Vector2.Distance(pixelCoords, newCoord) <= radius){

                        if(Raycounter % PixelsPerRay ==0){

                        Ray ray = mainCamera.ScreenPointToRay(new Vector3(newCoord.x, newCoord.y, 0f));
                        RaycastHit hit; 

                        //Debug.Log(newCoord + " " + Raycounter + " " + Timestamps[i]);

                        if(Physics.Raycast(ray, out hit)){

                         AllTimestamps.Add(Timestamps[i]);

                         GameObject hitObject = hit.collider.gameObject;
                         string Object = hitObject.name; 
                         AllObjecthit.Add(Object);

                         Vector3 hitWorldCoordinate = hit.point; 
                         AllObjecthitpos.Add(hitWorldCoordinate);


                        }
                    }

                    Raycounter++;

                  }
                
                }

            }
        }

        BinComputation(AllObjecthit, AllObjecthitpos, AllTimestamps);
    }

    System.Collections.IEnumerator MoveCameraThroughPositions(Vector3 camPosition, float camRotation){

            mainCamera.transform.position = new Vector3(camPosition.x, camPosition.y + 1.35f, camPosition.z);

            Vector3 newRotation = new Vector3 (12.5f, camRotation, 0f); 
            mainCamera.transform.eulerAngles = newRotation;

            yield return new WaitForSeconds(2.0f);


    }*/

    private void BinComputation(List<string> AllObjectname, List<Vector3> AllCoordinates, List<int> AllTimestamps){

     for(int i =0; i<AllTimestamps.Count; i++){

        string objname = AllObjectname[i];
        int timestamp = AllTimestamps[i];
        float xcoordinate = Mathf.Round(AllCoordinates[i].x * 1000f)/1000f;
        float ycoordinate = Mathf.Round(AllCoordinates[i].y * 1000f)/1000f;
        float zcoordinate = Mathf.Round(AllCoordinates[i].z * 1000f)/1000f;
        Vector3 pos = new Vector3 (xcoordinate,ycoordinate,zcoordinate);
        

        switch(objname){

        case "CueImage": 
                    binNum = 1;
                    break;
        case "HintImage": 
                    binNum = 2;
                    break;
        case "Ground": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Ground"], ObjToOffset);
                    break;

        case "Ceiling": 
                    int XBinNum = Mathf.CeilToInt(Mathf.Abs(pos.x - BottomLeftPos["Ceiling"].x)/binSize);
                    int ZBinNum = Mathf.CeilToInt(Mathf.Abs(pos.z - BottomLeftPos["Ceiling"].z)/binSize);
                    int binRow = Mathf.Clamp(ZBinNum, 0, numBinsWidth["Ceiling"]);
                    int binCol = Mathf.Clamp(XBinNum, 1, numBinsWidth["Ceiling"]);
                    binNum = (numBinsWidth["Ceiling"] - binRow) * numBinsWidth["Ceiling"] + binCol + ObjToOffset["Ceiling"];
                    break;

        case "wall_01":
        case "wall_02":
        case "wall_03": 
        case "wall_04":
        case "wall_05":
        case "wall_06":

                    int[] wallBinsWidth = walltag
                                        .Where(key => numBinsWidth.ContainsKey(key))
                                        .Select(key=> numBinsWidth[key])
                                        .ToArray();

                    binNum = WallTypeBinning(objname, pos, walltag, BottomLeftPos, binSize, wallBinsWidth, ObjToOffset, numBinsHeight);
                    break;

        case "m_wall_5": 
        case "m_wall_26": 
        case "m_wall_25":
        case "m_wall_1":
                     //Green pillars 
                        int[] greenPillarBinsWidth = greenpillartag
                                                .Where(key => numBinsWidth.ContainsKey(key))
                                                .Select(key => numBinsWidth[key])
                                                .ToArray();
                        binNum = MazeWallBinning(objname, pos, greenpillartag, BottomLeftPos, binSize, greenPillarBinsWidth, ObjToOffset, numBinsHeight);
                    break;
        
        case "m_wall_21": 
        case "m_wall_29":
        case "m_wall_6": 
        case "m_wall_10":
                    //Blue Pillars 
                    int[] bluePillarBinsWidth = bluepillartag
                                                .Where(key => numBinsWidth.ContainsKey(key))
                                                .Select(key=> numBinsWidth[key])
                                                .ToArray();
                    binNum = MazeWallBinning(objname, pos, bluepillartag, BottomLeftPos, binSize, bluePillarBinsWidth, ObjToOffset, numBinsHeight);
                    break; 
        
        case "m_wall_3": 
        case "m_wall_15": 
        case "m_wall_24": 
        case "m_wall_4":
                    //Red Pillars 
                    int[] redPillarBinsWidth = redpillartag
                                               .Where(key => numBinsWidth.ContainsKey(key))
                                               .Select(key => numBinsWidth[key])
                                               .ToArray();
                    binNum = MazeWallBinning(objname, pos, redpillartag, BottomLeftPos, binSize, redPillarBinsWidth, ObjToOffset, numBinsHeight);
                    break; 
                
        case "m_wall_20":
        case "m_wall_12": 
        case "m_wall_8": 
        case "m_wall_7":
                      //Yellow Pillars 
                    int[] yellowPillarBinsWidth = yellowpillartag
                                                  .Where(key => numBinsWidth.ContainsKey(key))
                                                  .Select(key => numBinsWidth[key])
                                                  .ToArray();
                    binNum = MazeWallBinning(objname, pos, yellowpillartag, BottomLeftPos, binSize, yellowPillarBinsWidth, ObjToOffset, numBinsHeight);
                    break;

                // Cat Cube Reward
        case "Plane_1": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_1"], ObjToOffset);
                    break;
        case "Plane_2":
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_2"], ObjToOffset);
                    break;
        case "Plane_3": 
        case "Plane_4":
        case "Plane_5":
        case "Plane_6": 
                      
                    int[] catPlaneBinsWidth =  catPlaneVertical
                                              .Where(key => numBinsWidth.ContainsKey(key))
                                              .Select(key => numBinsWidth[key])
                                              .ToArray();
                    binNum = WallTypeBinning(objname, pos, catPlaneVertical, BottomLeftPos, binSize, catPlaneBinsWidth, ObjToOffset, numBinsHeight);  
                    break; 

        case "Base_1": 
        case "Base_2": 
        case "Base_3": 
        case "Base_4": 
                    int[] catbaseBinsWidth = catbase
                                         .Where(key => numBinsWidth.ContainsKey(key))
                                         .Select(key=> numBinsWidth[key])
                                         .ToArray(); 
                    binNum = WallTypeBinning(objname, pos, catbase, BottomLeftPos, binSize, catbaseBinsWidth, ObjToOffset, numBinsHeight);
                    break;

                // Pig Cube Reward
        case "Plane_7": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_7"], ObjToOffset);
                    break; 
        case "Plane_8": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_8"], ObjToOffset);
                    break;
        case "Plane_9": 
        case "Plane_10": 
        case "Plane_11": 
        case "Plane_12":
                    int [] pigPlaneBinsWidth = pigPlaneVertical
                                               .Where(key => numBinsWidth.ContainsKey(key))
                                               .Select(key => numBinsWidth[key])
                                               .ToArray();
                    binNum = WallTypeBinning(objname, pos, pigPlaneVertical, BottomLeftPos, binSize, pigPlaneBinsWidth, ObjToOffset, numBinsHeight);
                    break;

        case "Base_5": 
        case "Base_6": 
        case "Base_7": 
        case "Base_8": 
                    int[] pigbaseBinsWidth = pigbase
                                         .Where(key => numBinsWidth.ContainsKey(key))
                                         .Select(key=> numBinsWidth[key])
                                         .ToArray(); 
                    binNum = WallTypeBinning(objname, pos, pigbase, BottomLeftPos, binSize, pigbaseBinsWidth, ObjToOffset, numBinsHeight);
                    break;

                //Crocodile Cube Reward
        case "Plane_13": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_13"], ObjToOffset); 
                    break; 
        case "Plane_14": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_14"], ObjToOffset);
                    break;
        case "Plane_15": 
        case "Plane_16":
        case "Plane_17": 
        case "Plane_18": 
                    int[] crocodilePlaneBinsWidth = crocodilePlaneVertical
                                                    .Where(key => numBinsWidth.ContainsKey(key))
                                                    .Select(key => numBinsWidth[key])
                                                    .ToArray();             
                    binNum = WallTypeBinning(objname, pos, crocodilePlaneVertical, BottomLeftPos, binSize, crocodilePlaneBinsWidth, ObjToOffset, numBinsHeight);
                    break; 
        case "Base_9": 
        case "Base_10": 
        case "Base_11": 
        case "Base_12": 
                    int[] crocodilebaseBinsWidth = crocodilebase
                                         .Where(key => numBinsWidth.ContainsKey(key))
                                         .Select(key=> numBinsWidth[key])
                                         .ToArray(); 
                    binNum = WallTypeBinning(objname, pos, crocodilebase, BottomLeftPos, binSize, crocodilebaseBinsWidth, ObjToOffset, numBinsHeight);
                    break;

                //Hippo Cube Reward
        case "Plane_19": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_19"], ObjToOffset); 
                    break; 
        case "Plane_20": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_20"], ObjToOffset); 
                    break; 
        case "Plane_21": 
        case "Plane_22": 
        case "Plane_23": 
        case "Plane_24": 
                    int[] hippoPlaneBinsWidth = hippoPlaneVertical
                                                .Where(key => numBinsWidth.ContainsKey(key))
                                                .Select(key => numBinsWidth[key])
                                                .ToArray();
                    binNum = WallTypeBinning(objname, pos, hippoPlaneVertical, BottomLeftPos, binSize, hippoPlaneBinsWidth, ObjToOffset, numBinsHeight);
                    break; 
        case "Base_13": 
        case "Base_14": 
        case "Base_15": 
        case "Base_16": 
                    int[] hippobaseBinsWidth = hippobase
                                         .Where(key => numBinsWidth.ContainsKey(key))
                                         .Select(key=> numBinsWidth[key])
                                         .ToArray(); 
                    binNum = WallTypeBinning(objname, pos, hippobase, BottomLeftPos, binSize, hippobaseBinsWidth, ObjToOffset, numBinsHeight);
                    break;

                // Rabbit Reward Cube
        case "Plane_25": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_25"], ObjToOffset);
                    break; 
        case "Plane_26": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_26"], ObjToOffset);
                    break; 
        case "Plane_27": 
        case "Plane_28": 
        case "Plane_29": 
        case "Plane_30": 
                    int[] rabbitPlaneBinsWidth = rabbitPlaneVertical
                                                .Where(key => numBinsWidth.ContainsKey(key))
                                                .Select(key => numBinsWidth[key])
                                                .ToArray();
                    binNum = WallTypeBinning(objname, pos, rabbitPlaneVertical, BottomLeftPos, binSize,rabbitPlaneBinsWidth, ObjToOffset, numBinsHeight);
                    break;

        case "Base_17": 
        case "Base_18": 
        case "Base_19": 
        case "Base_20": 
                    int[] rabbitbaseBinsWidth = rabbitbase
                                         .Where(key => numBinsWidth.ContainsKey(key))
                                         .Select(key=> numBinsWidth[key])
                                         .ToArray(); 
                    binNum = WallTypeBinning(objname, pos, rabbitbase, BottomLeftPos, binSize, rabbitbaseBinsWidth, ObjToOffset, numBinsHeight);
                    break;

                //Fox Cube Reward
        case "Plane_31": 
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_31"], ObjToOffset);
                    break; 
        case "Plane_32":
                    binNum = PlaneTypeBinning(objname, pos, binSize, numBinsWidth["Plane_32"], ObjToOffset); 
                    break; 
        case "Plane_33": 
        case "Plane_34":
        case "Plane_35": 
        case "Plane_36": 
                    int[] foxPlaneBinsWidth = foxPlaneVertical
                                              .Where(key => numBinsWidth.ContainsKey(key))
                                              .Select(key => numBinsWidth[key])
                                              .ToArray();
                    binNum = WallTypeBinning(objname, pos, foxPlaneVertical, BottomLeftPos, binSize, foxPlaneBinsWidth, ObjToOffset,numBinsHeight);
                    break;
        case "Base_21": 
        case "Base_22": 
        case "Base_23": 
        case "Base_24": 
                    int[] foxbaseBinsWidth = foxbase
                                         .Where(key => numBinsWidth.ContainsKey(key))
                                         .Select(key=> numBinsWidth[key])
                                         .ToArray(); 
                    binNum = WallTypeBinning(objname, pos, foxbase, BottomLeftPos, binSize, foxbaseBinsWidth, ObjToOffset, numBinsHeight);
                    break;

        default: // Gameobject is not found 
              binNum = -1;
                break;
        
        }

        if(binNum !=-1){

            Timestamps.Add(timestamp);
            Objecthitpos.Add(pos);
            Objecthit.Add(objname);
            Binnumbers.Add(binNum);
        }

        /*bool sameTimestamp = Timestamps.Contains(timestamp);
        bool sameBinnumber = Binnumbers.Contains(binNum);

        if(!Timestamps.Contains(timestamp)){  // Checks if the data of a given timestamp exists in the list 

            Timestamps.Add(AllTimestamps[i]);
            Binnumbers.Add(binNum);
            Objecthit.Add(AllObjectname[i]);
            Objecthitpos.Add(AllCoordinates[i]);
        }

        else if(Timestamps.Contains(timestamp)){  //If the timestamp already exist, it will check for the same bin number 

            if(sameTimestamp && !sameBinnumber){  // Bin numbers that are not replicated/same will be added to the list 

               Timestamps.Add(AllTimestamps[i]);
               Binnumbers.Add(binNum);
               Objecthit.Add(AllObjectname[i]);
               Objecthitpos.Add(AllCoordinates[i]);

            }

        }*/

        
    }

    /*for(int i=0; i< Timestamps.Count; i++ ){

        Debug.Log("Timestamp: " + Timestamps[i] + " " + "Bin Number: " + Binnumbers[i] + " "+ Objecthit[i] + " " + Objecthitpos[i]);
    }*/


     WriteCSV(Objecthit.ToArray(), Timestamps.ToArray(), Binnumbers.ToArray(), Objecthitpos.ToArray(), OutputFile);
        

}

void WriteCSV(string[] objecthit, int[] time, int[] binnum, Vector3[] gazecoordinate, string OutputFile){
        // Create a new StreamWriter to write to the file
        using (StreamWriter sw = new StreamWriter($"{OutputFile}{Path.DirectorySeparatorChar}data.csv"))
        {
            // Loop through the data list and write each array as a line in the CSV file
            for(int i =0; i< objecthit.Length; i++)
            {
                // Join the array elements with commas to create a CSV line
                string row = time[i] + "," + binnum[i] + "," + objecthit[i] + "," + gazecoordinate[i].x + "," + gazecoordinate[i].y + "," + gazecoordinate[i].z ;

                // Write the line to the file
                sw.WriteLine(row);
            }
        }

        Debug.Log("CSV file created: " + OutputFile);
    } 


public int WallTypeBinning(string objname, Vector3 position, string[] GroupObject, Dictionary<string, Vector3> BottomLeftPos, float binSize, int[] BinsWidth, Dictionary<string, int> ObjToOffset, Dictionary<string, int> BinsHeight){
        
        if(objname == GroupObject[0]){
            
                Vector3 distFromBottomLeft = position - BottomLeftPos[GroupObject[0]];
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.y)/binSize);
                int ZBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.z)/binSize);
                //int binRow = Mathf.Max(YBinNum-1 , 0); //Bin Row cannot be negative
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(ZBinNum, 1, BinsWidth[0]);
                binNum = binRow * BinsWidth.Sum() + binCol + ObjToOffset[GroupObject[0]];
                

        } else if (objname == GroupObject[1]){ 

                ObjToOffset[GroupObject[1]] = ObjToOffset[GroupObject[0]];
                Vector3 distFromBottomLeft = position - BottomLeftPos[GroupObject[1]];
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.y)/binSize);
                int XBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.x)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(XBinNum, 1, BinsWidth[1]);
                binNum =  binRow * BinsWidth.Sum() + binCol + BinsWidth[0] + ObjToOffset[GroupObject[1]];
           
        } else if(objname == GroupObject[2]){
            
                ObjToOffset[GroupObject[2]] = ObjToOffset[GroupObject[0]];
                Vector3 distFromBottomLeft = position - BottomLeftPos[GroupObject[2]]; 
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.y)/binSize);
                int ZBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.z)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(ZBinNum, 1, BinsWidth[2]);
                binNum = binRow * BinsWidth.Sum() + binCol + BinsWidth[0] + BinsWidth[1] + ObjToOffset[GroupObject[2]];

        } else if(objname == GroupObject[3]){
        
                ObjToOffset[GroupObject[3]] = ObjToOffset[GroupObject[0]];
                Vector3 distFromBottomLeft = position - BottomLeftPos[GroupObject[3]];
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.y)/binSize);
                int XBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.x)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(XBinNum, 1, BinsWidth[3]);
                binNum = binRow * BinsWidth.Sum() + binCol + BinsWidth[0] + BinsWidth[1] + BinsWidth[2] + ObjToOffset[GroupObject[3]];

    }else if(objname == GroupObject[4]){

                ObjToOffset[GroupObject[4]] = ObjToOffset[GroupObject[0]];
                Vector3 distFromBottomLeft = position - BottomLeftPos[GroupObject[4]];
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.y)/binSize);
                int ZBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.z)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(ZBinNum, 1, BinsWidth[4]);
                binNum = binRow * BinsWidth.Sum() + binCol + BinsWidth[0] + BinsWidth[1] + BinsWidth[2] + BinsWidth[3]+ ObjToOffset[GroupObject[4]];

    }else if(objname == GroupObject[5]){

                ObjToOffset[GroupObject[5]] = ObjToOffset[GroupObject[0]];
                Vector3 distFromBottomLeft = position - BottomLeftPos[GroupObject[5]];
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.y)/binSize);
                int XBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.x)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(XBinNum, 1, BinsWidth[5]);
                binNum = binRow * BinsWidth.Sum() + binCol + BinsWidth[0] + BinsWidth[1] + BinsWidth[2] + BinsWidth[3] + BinsWidth[4]+ ObjToOffset[GroupObject[5]];


    }

     return binNum;
  
}


public int MazeWallBinning(string objname, Vector3 position, string[] GroupObject, Dictionary<string, Vector3> BottomLeftPos, float binSize, int[] BinsWidth, Dictionary<string, int> ObjToOffset, Dictionary<string, int> BinsHeight)
    { //Binning for the maze wall uses a different method because it is binned starting from the right corner to the left corner 
      //In this case, the ref position is no longer from the bottom left corner

        if(objname == GroupObject[0]){

                Vector3 refposition = new Vector3(

                    BottomLeftPos[GroupObject[0]].x,
                    BottomLeftPos[GroupObject[0]].y, 
                    BottomLeftPos[GroupObject[0]].z - 5f
                    
                );

                //Debug.Log(refposition + objname);

                Vector3 distFromRef = position - refposition;
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromRef.y)/binSize);
                int ZBinNum = Mathf.CeilToInt(Mathf.Abs(distFromRef.z)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(ZBinNum, 1, BinsWidth[0]);
                binNum = binRow * BinsWidth.Sum() + binCol + ObjToOffset[GroupObject[0]];

        } else if (objname == GroupObject[1]){ 

                ObjToOffset[GroupObject[1]] = ObjToOffset[GroupObject[0]];

                Vector3 refposition = new Vector3(

                    BottomLeftPos[GroupObject[1]].x - 5f,
                    BottomLeftPos[GroupObject[1]].y, 
                    BottomLeftPos[GroupObject[1]].z  

                );

                //Debug.Log(refposition + objname);

                Vector3 distFromRef = position - refposition;
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromRef.y)/binSize);
                int XBinNum = Mathf.CeilToInt(Mathf.Abs(distFromRef.x)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(XBinNum, 1, BinsWidth[1]);
                binNum =  binRow * BinsWidth.Sum() + binCol + BinsWidth[0] + ObjToOffset[GroupObject[1]];
           
        } else if(objname == GroupObject[2]){
            
                ObjToOffset[GroupObject[2]] = ObjToOffset[GroupObject[0]];

                Vector3 refposition = new Vector3(

                    BottomLeftPos[GroupObject[2]].x, 
                    BottomLeftPos[GroupObject[2]].y, 
                    BottomLeftPos[GroupObject[2]].z + 5f

                );

                //Debug.Log(refposition + objname);

                Vector3 distFromRef = position - refposition; 
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromRef.y)/binSize);
                int ZBinNum = Mathf.CeilToInt(Mathf.Abs(distFromRef.z)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(ZBinNum, 1, BinsWidth[2]);
                binNum = binRow * BinsWidth.Sum() + binCol + BinsWidth[0] + BinsWidth[1] + ObjToOffset[GroupObject[2]];

        } else if(objname == GroupObject[3]){
        
                ObjToOffset[GroupObject[3]] = ObjToOffset[GroupObject[0]];
                
                Vector3 refposition = new Vector3( 

                    BottomLeftPos[GroupObject[3]].x + 5f, 
                    BottomLeftPos[GroupObject[3]].y,
                    BottomLeftPos[GroupObject[3]].z 

                );

                //Debug.Log(refposition + objname);

                Vector3 distFromRef = position - refposition;
                int YBinNum = Mathf.CeilToInt(Mathf.Abs(distFromRef.y)/binSize);
                int XBinNum = Mathf.CeilToInt(Mathf.Abs(distFromRef.x)/binSize);
                int binRow = Mathf.Clamp(YBinNum-1, 0, BinsHeight[objname]);
                int binCol = Mathf.Clamp(XBinNum, 1, BinsWidth[3]);
                binNum = binRow * BinsWidth.Sum() + binCol + BinsWidth[0] + BinsWidth[1] + BinsWidth[2] + ObjToOffset[GroupObject[3]];

        }

     return binNum;


    }


public int PlaneTypeBinning(string objname, Vector3 pos, float binSize, int ObjectWidth, Dictionary<string, int> ObjToOffset){

        Vector3 distFromBottomLeft = pos - BottomLeftPos[objname];
        int XBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.x)/binSize);
        int ZBinNum = Mathf.CeilToInt(Mathf.Abs(distFromBottomLeft.z)/binSize);
        //int binRow = Mathf.Max(ZBinNum-1 , 0); //Bin Row cannot be negative
        int binRow = Mathf.Clamp(ZBinNum-1, 0, ObjectWidth);
        int binCol = Mathf.Clamp(XBinNum, 1, ObjectWidth);
        binNum = binRow * ObjectWidth + binCol + ObjToOffset[objname];

        return binNum; 

}


}
