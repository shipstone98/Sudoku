//
//  ArraySudoku+Extensions.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

public extension ArraySudoku {
    init<T>(using generator: inout T) where T : RandomNumberGenerator {
        var solver = RecursiveSudokuSolver()
        solver.solve(using: &generator)
        self = solver.sudoku
    }
}

internal extension ArraySudoku {
    func candidates(_ row: Int, _ column: Int) -> Set<Int> {
        let peers = ArraySudoku.peers(row, column)
        var candidates: Set<Int> = []
        
        for candidate in 1...9 {
            let isSeen = peers.contains { self.array[$0] == candidate }
            
            if !isSeen {
                candidates.insert(candidate)
            }
        }
        
        return candidates
    }
    
    static func peers(_ row: Int, _ column: Int) -> Set<Int> {
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
}
