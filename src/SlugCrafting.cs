using RWCustom;
using UnityEngine;

namespace SlugCrafting;

public class SlugCrafting
{

    //
    // SPAWNING UTILITY FUNCTIONS
    //

    /// <summary>
    /// Spawns a physical object in the room and position of another physical object.
    /// Generic, allows for any type of physical object to be spawned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spawningBodyChunk"></param>
    /// <param name="abstractObjectFactory"></param>
    /// <returns></returns>
    public static T AddPhysicalObjectOnBodyChunkFromAbstract<T>(
    in BodyChunk spawningBodyChunk,
    in Func<World, WorldCoordinate, EntityID, AbstractPhysicalObject> abstractObjectFactory
    ) where T : AbstractPhysicalObject
    {
        var spawnOnObject = spawningBodyChunk.owner as PhysicalObject;
        // Convert world position to tile coordinates
        var tilePosition = spawnOnObject.room.GetTilePosition(spawningBodyChunk.pos);
        var worldCoordinate = new WorldCoordinate(spawnOnObject.room.abstractRoom.index, tilePosition.x, tilePosition.y, 0);

        // Create the abstract object using the provided factory
        var abstractObj = abstractObjectFactory(spawnOnObject.room.world, worldCoordinate, spawnOnObject.room.game.GetNewID());

        // Realize the object (convert to concrete)
        abstractObj.Realize();

        // Register in the room
        spawnOnObject.room.abstractRoom.AddEntity(abstractObj);
        spawnOnObject.room.AddObject(abstractObj.realizedObject);

        // Return the realized object as type T
        return (T)abstractObj;
    }

    /// <summary>
    /// Spawns a physical object in the room and position of another physical object.
    /// Generic, allows for any type of physical object to be spawned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spawningCreature"></param>
    /// <param name="abstractObjectFactory"></param>
    /// <returns></returns>
    public static T AddPhysicalObjectOnCreatureFromAbstract<T>(
    in Creature spawnOnCreature,
    in Func<World, WorldCoordinate, EntityID, AbstractPhysicalObject> abstractObjectFactory
    ) where T : AbstractPhysicalObject
    {
        // Convert world position to tile coordinates
        var tilePosition = spawnOnCreature.room.GetTilePosition(spawnOnCreature.mainBodyChunk.pos);
        var worldCoordinate = new WorldCoordinate(spawnOnCreature.room.abstractRoom.index, tilePosition.x, tilePosition.y, 0);

        // Create the abstract object using the provided factory
        var abstractObj = abstractObjectFactory(spawnOnCreature.room.world, worldCoordinate, spawnOnCreature.room.game.GetNewID());

        // Realize the object (convert to concrete)
        abstractObj.Realize();

        // Register in the room
        spawnOnCreature.room.abstractRoom.AddEntity(abstractObj);
        spawnOnCreature.room.AddObject(abstractObj.realizedObject);

        // Return the realized object as type T
        return (T)abstractObj;
    }

    /// <summary>
    /// Spawns a physical object in the specified room and position.
    /// Generic, allows for any type of physical object to be spawned.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="room"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="abstractObjectFactory"></param>
    /// <returns></returns>
    public static T AddPhysicalObjectFromAbstract<T>(
        in Room room,
        in Vector2 spawnPosition,
        in Func<World, WorldCoordinate, EntityID, AbstractPhysicalObject> abstractObjectFactory
    ) where T : AbstractPhysicalObject
    {
        // Convert world position to tile coordinates
        var tilePosition = room.GetTilePosition(spawnPosition);
        var worldCoordinate = new WorldCoordinate(room.abstractRoom.index, tilePosition.x, tilePosition.y, 0);

        // Create the abstract object using the provided factory
        var abstractObj = abstractObjectFactory(room.world, worldCoordinate, room.game.GetNewID());

        // Realize the object (convert to concrete)
        abstractObj.Realize();

        // Register in the room
        room.abstractRoom.AddEntity(abstractObj);
        room.AddObject(abstractObj.realizedObject);

        // Return the realized object as type T
        return (T)abstractObj;
    }
}
