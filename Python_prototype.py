#started 05/09/19


import pygame as py
from pygame.locals import *

#start pygame
py.init()

#colours
black = (0,0,0)
white = [255,255,255]

#start the screen
screen_size = (800,600)
screen = py.display.set_mode(screen_size)

#tile class, this is the tile
class Tile:
    #size of the tile, make it the same for all tiles
    length = 50
    
    #initialize variables
    def __init__(self,x,y,floor):
        self.x = x
        self.y = y
        self.name = ""
        self.floor = floor
    
    #draw the tile
    def draw(self,surf):
        py.draw.rect(surf,white + [255],((6*self.floor + self.x) * self.length,100 + self.y * self.length, self.length,self.length))

#the game class, this handles everyhting needed for the game
class Game:
    
    #initialize the variables
    def __init__(self):
        self.scene = 0
        attic = ["Attic",Tile(0,1,2)]
        ground = ["Ground Floor",Tile(5,3,1)]
        basement = ["Basement",Tile(1,2,0)]
        self.house = [basement,ground,attic]
        self.zoom = 1
    
    #draw the board of tiles
    def Draw_board(self):
        surf = py.Surface((900,600),py.SRCALPHA)
        #for each floor, draw it
        for i,floor in enumerate(self.house):
            Win.DrawText(floor[0],800 * ((1/6)*(2*i+1)),50,Win.sys_font,surf = surf)
            for tile in floor[1:]:
                tile.draw(surf)
        obj = py.transform.scale(surf,(
            int(surf.get_width() * self.zoom), int(surf.get_height() * self.zoom))
                                 )
        screen.blit(obj,(400-obj.get_width()//2,300-obj.get_height()//2))


#game class, handles events, drawing, the window
class Window:
    
    game = Game()
    
    #initialize system variables
    def __init__(self):
        self.running = True
        self.click = False
        self.MouseX, self.MouseY = 0,0
        self.sys_font = py.font.Font(py.font.match_font("Calibri"),20)
        self.clock = py.time.Clock()
        self.scrolling = 0
    
    #once game starts
    def Start(self):
        #run the game, updating the window at every frame
        while self.running:
            self.Update() 
    
    #this is called every frame, handles what happens every frame
    def Update(self):
        
        self.clock.tick(100)
        
        #draw the screen
        self.Draw()
        #update the screen
        py.display.update()
        #update system variables
        self.MouseX, self.MouseY = py.mouse.get_pos()
        #reset them for new frame
        self.click = False
        self.scrolling = 0
        #get events
        for e in py.event.get():
            if e.type == 12:
                py.quit()
                self.running = False
            if e.type == MOUSEBUTTONDOWN:
                self.click = True if e.button < 3 else False
                self.scrolling = 1 if e.button == 4 else -1 if e.button == 5 else 0
                self.game.zoom += self.scrolling * 0.01
            
    
    #an easier way to put text on screen
    def DrawText(self,txt,x,y,font,center = True,surf = screen):
        obj = font.render(txt,True,white)
        w = obj.get_width()
        h = obj.get_height()
        x -= w//2 if center else 0
        y -= h//2 if center else 0
        surf.blit(obj,(x,y))
    
    
    #draw the window
    def Draw(self):
        screen.fill(black)
        self.game.Draw_board()

Win = Window()
Win.Start()