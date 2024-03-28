# RiskAnalyzer
AR/VR project that, thanks to image recognition by AI, allows analyzing the risks present in the user's field of vision.

This project is a demo of the prototype created during the 2024 edition of the student Hackathon organized by MIC (Meet Innovate Create).

This application is designed to assist visually impaired individuals in identifying risks around them using Text-to-Speech and the future collaboration between Meta and Ray-Ban.

The application could also be applied in B2B scenarios for security systems that would alert the police if a suspicious person is detected or notify firefighters if a fire breaks out in a house.

This demo allows : 

- To capture the screen viewed by the user in the Meta Quest Pro.
- To send capture to the Microsft Azure OpenAI Vision (https://learn.microsoft.com/en-us/azure/ai-services/openai/).
- To show responses from AI in the Meta Quest Pro.

Potential improvements :

- Send various requests to AI with vocal commands or inputs management on gamepad.
- Implements the Text-to-Speech functionnality.
- Take screen capture regularly or at desired times.
- Analyze risks in an AR environnement.
- Expand the project's potential use cases.

  How it works :

  - We retrieve the screen from Meta Quest Pro with SideQuest app (https://sidequestvr.com/setup-howto)
  - We use an Unity asset to capture windows and make them available in Unity as Texture2D and show SideQuest stream in Unity. (https://github.com/hecomi/uWindowCapture)
  - We send the capture of this Texture2D as a Base64 image to AI.
  - We receive and convert the response to display it in the headset on a panel containing a Text Mesh Pro component.
 
  Assets used : 
  - RPG Poly Pack - Lite (https://assetstore.unity.com/packages/3d/environments/landscapes/rpg-poly-pack-lite-148410)
  - FREE Stylized Bear - RPG Forest Animal (https://assetstore.unity.com/packages/3d/characters/animals/free-stylized-bear-rpg-forest-animal-228910)
  - Free Fire VFX - URP (https://assetstore.unity.com/packages/vfx/particles/fire-explosions/free-fire-vfx-urp-266226)
  - Sedan car - 01 (https://assetstore.unity.com/packages/3d/vehicles/land/sedan-car-01-190629)
