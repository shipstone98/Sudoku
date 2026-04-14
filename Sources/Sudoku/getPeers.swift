//
//  getPeers.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 02/04/2026.
//

internal func getPeers(_ row: Int, _ column: Int) -> Set<Int> {
    var peers: Set<Int> = []
    
    for index in 0..<9 {
        peers.insert(row * 9 + index)
        peers.insert(index * 9 + column)
    }
    
    let blockRow = row - row % 3
    let blockColumn = column - column % 3
    
    for rowOffset in 0..<3 {
        let currentRow = blockRow + rowOffset
        
        for columnOffset in 0..<3 {
            peers.insert(currentRow * 9 + blockColumn + columnOffset)
        }
    }
    
    peers.remove(row * 9 + column)
    return peers
}
