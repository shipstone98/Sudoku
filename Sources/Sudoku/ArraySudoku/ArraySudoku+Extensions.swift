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
        let peers = getPeers(row, column)
        var candidates: Set<Int> = []
        
        for candidate in 1...9 {
            let isSeen = peers.contains { self.array[$0] == candidate }
            
            if !isSeen {
                candidates.insert(candidate)
            }
        }
        
        return candidates
    }
}
